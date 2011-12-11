using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace BNHelperX
{
    public partial class Form1 : Form
    {
        int GameState = 0;
        bool RoomFull = false;
        System.Threading.Timer SendEnterTimer, InjectDllTimer, RoomFullTimer;
        Thread AutoTryThread;

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilter(uint msg, uint flags);

        [DllImport("user32.dll")]
        static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool InsertMenu(IntPtr hmenu, uint position, uint flags,
            uint item_id, [MarshalAs(UnmanagedType.LPTStr)]string item_text);

        [DllImport("user32.dll")]
        static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        [DllImport("user32.dll")]
        static extern bool ModifyMenu(IntPtr hMnu, uint uPosition, uint uFlags,
            UIntPtr uIDNewItem, string lpNewItem);

        [DllImport("user32.dll")]
        static extern uint CheckMenuItem(IntPtr hmenu, uint uIDCheckItem, uint uCheck);

        public Form1()
        {
            InitializeComponent();

            IntPtr hMenu = GetSystemMenu(this.Handle, false);
            InsertMenu(hMenu, 0, 0x00000408, 0x123, "总在最前");
            InsertMenu(hMenu, 0, 0x00000000, 0x124, "版本v3.2  点这里访问网站");

            this.TopMost = true;

            this.listViewPlayerList.Columns.Add("位置", -2, HorizontalAlignment.Left);
            this.listViewPlayerList.Columns.Add("玩家名", 105, HorizontalAlignment.Left);
            this.listViewPlayerList.Columns.Add("队伍", -2, HorizontalAlignment.Left);
            this.listViewPlayerList.Columns.Add("IP地址", 105, HorizontalAlignment.Left);
            this.listViewPlayerList.Columns.Add("地理位置", -2, HorizontalAlignment.Left);

            this.listViewAdvList.Columns.Add("游戏名", -2, HorizontalAlignment.Left);
            this.listViewAdvList.Columns.Add("地图", -2, HorizontalAlignment.Left);
            this.listViewAdvList.Columns.Add("创建者", -2, HorizontalAlignment.Left);
            this.listViewAdvList.Columns.Add("IP地址", -2, HorizontalAlignment.Left);
            this.listViewAdvList.Columns.Add("地理位置", -2, HorizontalAlignment.Left);

            //this.listBoxMessage.Items.Add("主要功能:");
            //this.listBoxMessage.Items.Add("+魔兽外主机列表,自动进主 (BN Only)");
            //this.listBoxMessage.Items.Add("+魔兽外房间玩家列表、聊天信息");
            //this.listBoxMessage.Items.Add("+玩家ip和地理位置查询");
            //this.listBoxMessage.Items.Add("+视野外点选(开图)探测 (此功能存在误判,谨慎对待)");
            //this.listBoxMessage.Items.Add("目前支持魔兽版本:1.24b,1.24e");

            SendEnterTimer = new System.Threading.Timer(new TimerCallback(SendEnter), null, Timeout.Infinite, Timeout.Infinite);
            InjectDllTimer = new System.Threading.Timer(new TimerCallback(InjectDll), null, Timeout.Infinite, Timeout.Infinite);
            RoomFullTimer = new System.Threading.Timer(new TimerCallback(RoomFullActive), null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                try
                {
                    ChangeWindowMessageFilter(0x004A, 1); // enable WM_COPYDATA message
                }
                catch { }
            }

            ChangePrivilege();

            InjectDllTimer.Change(0, 4000);
        }

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool OpenProcessToken(IntPtr ProcessHandle, UInt32 DesiredAccess, out IntPtr TokenHandle);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool LookupPrivilegeValue(string SystemName, string Name, out long Luid);

        [DllImport("advapi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AdjustTokenPrivileges(IntPtr TokenHandle,
           [MarshalAs(UnmanagedType.Bool)] bool DisableAllPrivileges,
           ref TokenPrivileges NewState,
           UInt32 Zero,
           IntPtr Null1,
           IntPtr Null2);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct TokenPrivileges
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        private void ChangePrivilege()
        {
            try
            {
                IntPtr TokenHandle;
                TokenPrivileges tp;
                tp.Count = 1;
                tp.Luid = 0;
                tp.Attr = 0x00000002;
                if (OpenProcessToken(GetCurrentProcess(), 0x0020 | 0x0008, out TokenHandle))
                {
                    if (LookupPrivilegeValue(null, "SeDebugPrivilege", out tp.Luid))
                    {
                        if (AdjustTokenPrivileges(TokenHandle, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero))
                        {
                            // 成功
                            //MessageBox.Show("Success!");
                        }
                    }
                }
            }
            catch
            {

            }
        }

        [DllImport("kernel32.dll")]
        static extern int VirtualAllocEx(IntPtr hwnd, Int32 lpaddress, int size, int type, Int32 tect);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool WriteProcessMemory(IntPtr hwnd, int baseaddress, string buffer, int nsize, int filewriten);

        [DllImport("kernel32.dll")]
        static extern int GetProcAddress(int hwnd, string lpname);

        [DllImport("kernel32.dll")]
        static extern int GetModuleHandleA(string name);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateRemoteThread(IntPtr hwnd, int attrib, int size, int address, int par, int flags, int threadid);

        private void InjectDll(object obj)
        {
            try
            {
                string DllFilePath = Application.StartupPath + "\\BNHelper.dll";
                Process[] ps = Process.GetProcessesByName("war3");
                foreach (Process p in ps)
                {
                    IntPtr hwnd = p.Handle;
                    bool loaded = false;
                    foreach (ProcessModule m in p.Modules)
                    {
                        if (m.ModuleName == "BNHelper.dll")
                        {
                            return;
                        }
                        if (m.ModuleName == "Game.dll")
                        {
                            loaded = true;
                        }
                    }
                    if (!loaded)
                        return;

                    Thread.Sleep(2000);

                    int BaseAddress = VirtualAllocEx(hwnd, 0, DllFilePath.Length * 2 + 1, 0x1000, 0x04);
                    if (BaseAddress != 0)
                    {
                        if (WriteProcessMemory(hwnd, BaseAddress, DllFilePath, DllFilePath.Length * 2 + 1, 0))
                        {
                            int FunctionAddress = GetProcAddress(GetModuleHandleA("kernel32.dll"), "LoadLibraryW");
                            CreateRemoteThread(hwnd, 0, 0, FunctionAddress, BaseAddress, 0, 0);
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private string FormatMessage(string msg)
        {
            string result = msg.Replace("|r", "");
            result = result.Replace("|R", "");
            int k;
            while ((k = result.IndexOf("|C")) >= 0)
            {
                result = result.Remove(k, 10);
            }
            while ((k = result.IndexOf("|c")) >= 0)
            {
                result = result.Remove(k, 10);
            }
            return result;
        }

        private void UpdateMessage(string msg)
        {
            bool scroll = false;
            if (this.listBoxMessage.TopIndex == this.listBoxMessage.Items.Count - (int)(this.listBoxMessage.Height / this.listBoxMessage.ItemHeight))
                scroll = true;
            this.listBoxMessage.Items.Add(msg);
            if (scroll)
                this.listBoxMessage.TopIndex = this.listBoxMessage.Items.Count - (int)(this.listBoxMessage.Height / this.listBoxMessage.ItemHeight);
        }

        private void UpdatePlayerList(ROOM room)
        {
            RoomFull = true;
            this.listViewPlayerList.BeginUpdate();
            this.listViewPlayerList.Items.Clear();
            for (int i = 0; i < room.count; ++i)
            {
                ListViewItem item = new ListViewItem((i + 1).ToString());
                string name = "";
                if (room.line[i].emptyOrClosed == 0)
                {
                    name = "";
                    RoomFull = false;
                }
                else if (room.line[i].emptyOrClosed == 1)
                {
                    name = "关闭";
                }
                else
                {
                    if (room.line[i].humanOrAi == 1)
                    {
                        if (room.line[i].aiLevel == 0)
                        {
                            name = "电脑(简单的)";
                        }
                        else if (room.line[i].aiLevel == 1)
                        {
                            name = "电脑(中等的)";
                        }
                        else
                        {
                            name = "电脑(发狂的)";
                        }
                    }
                    else
                    {
                        name = room.player[room.line[i].playerId].name;
                        if (room.line[i].mapProcess == 0xFF)
                        {
                            name = "[?]" + name;
                        }
                        else if (room.line[i].mapProcess < 100)
                        {
                            name = "[" + room.line[i].mapProcess + "%]" + name;
                        }
                    }
                }
                
                item.SubItems.Add(name);
                item.SubItems.Add((room.line[i].team + 1).ToString());
                item.SubItems.Add(room.player[room.line[i].playerId].ip);
                item.SubItems.Add(room.player[room.line[i].playerId].location);
                this.listViewPlayerList.Items.Add(item);
            }

            this.listViewPlayerList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewPlayerList.EndUpdate();

            if (room.host && RoomFull)
            {
                RoomFullTimer.Change(3000, Timeout.Infinite);
            }
        }

        void RoomFullActive(object obj)
        {
            if (GameState == 1 && RoomFull)
            {
                ActiveWC3();
            }
        }

        private void UpdateAdvList(ADVLIST advList)
        {
            this.listViewAdvList.BeginUpdate();
            this.listViewAdvList.Items.Clear();
            for (int i = 0; i < advList.count; ++i)
            {
                ListViewItem item = new ListViewItem(advList.adv[i].title);
                int k = advList.adv[i].map.LastIndexOf('\\');
                if (k >= 0)
                    advList.adv[i].map = advList.adv[i].map.Substring(k + 1);
                item.SubItems.Add(advList.adv[i].map);
                item.SubItems.Add(advList.adv[i].creator);
                item.SubItems.Add(advList.adv[i].ip);
                item.SubItems.Add(advList.adv[i].location);
                this.listViewAdvList.Items.Add(item);
            }
            this.listViewAdvList.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            this.listViewAdvList.EndUpdate();
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LINE
        {
            public byte playerId;
            public byte mapProcess;
            public byte emptyOrClosed;
            public byte humanOrAi;
            public byte team;
            public byte color;
            public byte race;
            public byte aiLevel;
            public byte hp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECORD
        {
            public int win;
            public int lose;
            public int kill;
            public int dead;
            public int assist;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct PLAYER
        {
            public RECORD record;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string name;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string ip;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string location;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct ROOM
        {
            public byte count;
            public bool host;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
            public LINE[] line;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 13)]
            public PLAYER[] player;
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ADV
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 35)]
            public string title;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string creator;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
            public string ip;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string location;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
            public string map;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ADVLIST
        {
            public int count;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public ADV[] adv;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x004A)
            {
                try
                {
                    COPYDATASTRUCT cds = new COPYDATASTRUCT();
                    Type cdsType = cds.GetType();
                    cds = (COPYDATASTRUCT)m.GetLParam(cdsType);
                    uint flag = (uint)cds.dwData;
                    byte[] data = new byte[cds.cbData];
                    Marshal.Copy(cds.lpData, data, 0, cds.cbData);

                    if (flag == 1)
                    {
                        UpdateMessage(Encoding.Unicode.GetString(data));
                    }
                    else if (flag == 2)
                    {
                        int size = Marshal.SizeOf(typeof(ROOM));
                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(data, 0, ptr, size);
                        ROOM room = (ROOM)Marshal.PtrToStructure(ptr, typeof(ROOM));
                        Marshal.FreeHGlobal(ptr);

                        UpdatePlayerList(room);

                        this.panelInRoom.Visible = true;
                        this.panelOutRoom.Visible = false;
                    }
                    else if (flag == 3)
                    {
                        int size = Marshal.SizeOf(typeof(ADVLIST));
                        IntPtr ptr = Marshal.AllocHGlobal(size);
                        Marshal.Copy(data, 0, ptr, size);
                        ADVLIST advList = (ADVLIST)Marshal.PtrToStructure(ptr, typeof(ADVLIST));
                        Marshal.FreeHGlobal(ptr);

                        UpdateAdvList(advList);

                        this.panelInRoom.Visible = false;
                        this.panelOutRoom.Visible = true;

                        if (this.checkBoxAutoJoin.Checked)
                        {
                            for (int i = 0; i < advList.count; ++i)
                            {
                                if (advList.adv[i].title.IndexOf(this.textBoxTitleContain.Text, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                    advList.adv[i].map.IndexOf(this.textBoxMapContain.Text, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    JoinGame(advList.adv[i].title);
                                }
                            }
                        }
                    }
                    else if (flag == 4)
                    {
                        //W3_NOTHING 0
                        //W3_LOBBY 1
                        //W3_FAKE 2 // i don't know what is this...
                        //W3_LOADING 3
                        //W3_INGAME 4
                        //W3_PAUSEDGAME 6
                        int backupGameState = GameState;
                        GameState = BitConverter.ToInt32(data, 0);
                        if (backupGameState == 1 && GameState == 0) // 从房间中退出
                        {
                            UpdateMessage("[已离开房间]");
                            ActiveWC3();
                        }
                        else if (GameState == 1) // 进入房间
                        {
                            this.listBoxMessage.Items.Clear();
                            this.panelInRoom.Visible = true;
                            this.panelOutRoom.Visible = false;
                            this.checkBoxAutoJoin.Checked = false;
                            this.checkBoxAutoTry.Checked = false;
                            ActiveWC3();
                            
                        }
                        else if (GameState == 3) // 开始载入
                        {
                            UpdateMessage("[已开始载入]");
                        }
                        else if (GameState == 4) // 开始游戏
                        {
                            UpdateMessage("[已开始游戏]");
                            ActiveWC3();
                        }
                    }
                }
                catch (Exception e)
                {
                    //MessageBox.Show(e.ToString());
                }
            }
            else if (m.Msg == 0x0112)
            {
                if (m.WParam.ToInt32() == 0x123)
                {
                    IntPtr hMenu = GetSystemMenu(this.Handle, false);
                    CheckMenuItem(hMenu, 0x123, this.TopMost ? (uint)0 : 0x00000008);
                    this.TopMost = !this.TopMost;
                }
                else if (m.WParam.ToInt32() == 0x124)
                {
                    System.Diagnostics.Process.Start("http://blog.nxun.com/2011/war3-bn-helper/");
                }
                else
                {
                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        void ActiveWC3()
        {

            IntPtr hwnd;
            if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
            {
                if (hwnd != GetForegroundWindow())
                {
                    SendMessage(hwnd, 0x0112, 0xF030, 0);
                }
            }
        }

        //void GoToAdvList(object obj)
        //{
        //    IntPtr hwnd;
        //    if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
        //    {
        //        SendMessage(hwnd, 0x0100, 0x12, 0); // Alt + G
        //        SendMessage(hwnd, 0x0100, 0x47, 0);
        //        SendMessage(hwnd, 0x0101, 0x47, 0);
        //        SendMessage(hwnd, 0x0101, 0x12, 0);
        //    }
        //}

        void SendEnter(object obj)
        {
            IntPtr hwnd;
            if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
            {
                SendMessage(hwnd, 0x0100, 0x0d, 0); // Enter
                SendMessage(hwnd, 0x0101, 0x0d, 0);
            }
        }

        void JoinGame(string title)
        {
            IntPtr hwnd;
            if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
            {
                byte[] b = new byte[Encoding.UTF8.GetByteCount(title) + 1];
                Encoding.UTF8.GetBytes(title).CopyTo(b, 0);

                Clipboard.Clear();
                Clipboard.SetData(DataFormats.Text, new MemoryStream(b));

                SendMessage(hwnd, 0x0100, 0x10, 0); // Shift + Tab + Tab
                SendMessage(hwnd, 0x0100, 0x09, 0);
                SendMessage(hwnd, 0x0101, 0x09, 0);
                SendMessage(hwnd, 0x0100, 0x09, 0);
                SendMessage(hwnd, 0x0101, 0x09, 0);
                SendMessage(hwnd, 0x0101, 0x10, 0);

                //SendMessage(hwnd, 0x0100, 0x10, 0); // Shift + Home + End
                //SendMessage(hwnd, 0x0100, 0x24, 0);
                //SendMessage(hwnd, 0x0101, 0x24, 0);
                //SendMessage(hwnd, 0x0100, 0x23, 0);
                //SendMessage(hwnd, 0x0101, 0x23, 0);
                //SendMessage(hwnd, 0x0101, 0x10, 0);

                //SendMessage(hwnd, 0x0100, 0x2E, 0); // Delete
                //SendMessage(hwnd, 0x0101, 0x2E, 0);

                SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl + V
                SendMessage(hwnd, 0x0100, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x11, 0);

                SendMessage(hwnd, 0x0100, 0x09, 0); // Tab
                SendMessage(hwnd, 0x0101, 0x09, 0);

                SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl + V
                SendMessage(hwnd, 0x0100, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x11, 0);

                SendMessage(hwnd, 0x0100, 0x09, 0); // Tab
                SendMessage(hwnd, 0x0101, 0x09, 0);

                SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl + V
                SendMessage(hwnd, 0x0100, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x11, 0);

                SendMessage(hwnd, 0x0100, 0x09, 0); // Tab
                SendMessage(hwnd, 0x0101, 0x09, 0);

                SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl + V
                SendMessage(hwnd, 0x0100, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x11, 0);

                SendMessage(hwnd, 0x0100, 0x09, 0); // Tab
                SendMessage(hwnd, 0x0101, 0x09, 0);

                SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl + V
                SendMessage(hwnd, 0x0100, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x56, 0);
                SendMessage(hwnd, 0x0101, 0x11, 0);
                
                SendMessage(hwnd, 0x0100, 0x12, 0); // Alt + J
                SendMessage(hwnd, 0x0100, 0x4A, 0);
                SendMessage(hwnd, 0x0101, 0x4A, 0);
                SendMessage(hwnd, 0x0101, 0x12, 0);
                                
                //SendMessage(hwnd, 0x0100, 0x0d, 0); // Enter 防止房间名错误进程卡住
                //SendMessage(hwnd, 0x0101, 0x0d, 0);

                SendEnterTimer.Change(200, Timeout.Infinite);
            }
        }


        [DllImport("user32.dll")]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        static extern void SendMessage(IntPtr hWnd, uint msg, uint wParam, uint lParam);

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (GameState == 1) // 只在房间中发送
                {
                    IntPtr hwnd;
                    if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
                    {
                        byte[] b = new byte[Encoding.UTF8.GetByteCount(this.textBoxChat.Text) + 1];
                        Encoding.UTF8.GetBytes(this.textBoxChat.Text).CopyTo(b, 0);

                        Clipboard.SetData(DataFormats.Text, new MemoryStream(b));

                        SendMessage(hwnd, 0x0100, 0x11, 0); // Ctrl KeyDown
                        SendMessage(hwnd, 0x0100, 0x56, 0); // V KeyDown
                        SendMessage(hwnd, 0x0101, 0x56, 0); // V KeyUp
                        SendMessage(hwnd, 0x0101, 0x11, 0); // Ctrl KeyUp
                        SendMessage(hwnd, 0x0100, 0x0d, 0); // Enter KeyDown
                        SendMessage(hwnd, 0x0101, 0x0d, 0); // Enter KeyUp

                        this.textBoxChat.Clear();
                    }
                }
                e.Handled = true;
            }
        }

        private void listBoxMessage_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index >= 0 && e.Index < this.listBoxMessage.Items.Count)
            {
                Brush brush = Brushes.Black;
                if (this.listBoxMessage.Items[e.Index].ToString().StartsWith("|CFFAAAAAA"))
                {
                    brush = Brushes.DarkGray;
                }
                e.Graphics.DrawString(FormatMessage(listBoxMessage.Items[e.Index].ToString()), e.Font, brush, e.Bounds, StringFormat.GenericDefault);
            }
            e.DrawFocusRectangle();
        }

        private void listViewAdvList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = this.listViewAdvList.HitTest(e.X, e.Y);
            if (hit.Item != null)
            {
                JoinGame(hit.Item.Text);
            }
        }

        private void checkBoxAutoJoin_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAutoJoin.Checked == true)
            {
                this.checkBoxAutoTry.Checked = false;
            }
        }

        private void checkBoxAutoTry_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBoxAutoTry.Checked == true)
            {
                this.checkBoxAutoJoin.Checked = false;
                AutoTryThread = new Thread(new ThreadStart(AutoTry));
                AutoTryThread.SetApartmentState(ApartmentState.STA);
                AutoTryThread.Start();
            }
            else
            {
                AutoTryThread.Abort();
            }
        }

        private void AutoTry()
        {
            JoinGame(this.textBoxTitleIs.Text);
            Thread.Sleep(300);

            IntPtr hwnd;
            if ((hwnd = FindWindow("Warcraft III", "Warcraft III")) != IntPtr.Zero)
            {
                while (true)
                {
                    SendMessage(hwnd, 0x0100, 0x12, 0); // Alt + J
                    SendMessage(hwnd, 0x0100, 0x4A, 0);
                    SendMessage(hwnd, 0x0101, 0x4A, 0);
                    SendMessage(hwnd, 0x0101, 0x12, 0);

                    Thread.Sleep(200);

                    SendMessage(hwnd, 0x0100, 0x0d, 0); // Enter
                    SendMessage(hwnd, 0x0101, 0x0d, 0);

                    Thread.Sleep(100);
                }
            }

        }
    }

    /// <summary>
    /// This override list view support double buffer to avoid the twinkling when insert a new item dynamically
    /// </summary>
    public class DoubleBufferListView : ListView
    {
        public DoubleBufferListView()
        {
            this.DoubleBuffered = true;
        }
    }
}
