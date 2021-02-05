Imports System.IO
Imports System.Net
Imports System.Threading
Public Class Form1
    Private Delegate Sub AddMessageHandler(ByVal msg As String)
    Dim server As New ProcessStartInfo("cmd.exe")
    Private _process As Process = Nothing
    Dim dwlurl, dwlfilename, serverurl
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        MsgBox("自己填进去服务端目录", 64, "自己填")
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click

        server.WindowStyle = ProcessWindowStyle.Hidden
        server.Arguments = "/c @ECHO OFF &&" & " java -jar " & serverurl & " nogui" & ComboBox1.Text
        server.WorkingDirectory = Application.StartupPath & "/MinecraftServer"
        server.UseShellExecute = False
        server.RedirectStandardOutput = True
        server.RedirectStandardInput = True
        server.CreateNoWindow = True
        _process = New Process()
        _process.StartInfo = server
        ' 定义接收消息的Handler
        AddHandler _process.OutputDataReceived, New DataReceivedEventHandler(AddressOf Process1_OutputDataReceived)
        _process.Start()
        ' 开始接收
        _process.BeginOutputReadLine()
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            _process.Close()
        Catch ex As Exception
            MsgBox(ex.Message, 16, "进程结束失败！（别点击那么多次）")
        End Try
    End Sub

    Public Sub xzq1()
        Dim hwq As HttpWebRequest
        Dim hwp As HttpWebResponse
        Dim colHeader As WebHeaderCollection  '响应头信息集合
        Dim lngSize As Int64                  '要下载文件的总大小
        Dim lngCurSize As Int64               '已经下载大小
        Dim lngNet As Int64                   '计算网速用

        Dim stRespones As Stream              '响应流
        Dim st As FileStream                  '本地流
        Dim intCurSize As Int64
        Dim bytBuffer(512) As Byte           '缓存大小

        Dim datLast As DateTime               '最后一次时间
        Dim intDiff As Int32                  '两次时间差（秒）

        datLast = Now   '取得开始时间
        hwq = CType(HttpWebRequest.Create(dwlurl), HttpWebRequest) '请求对象创建
        hwp = hwq.GetResponse        '取得响应对象
        colHeader = hwp.Headers      '取得响应头
        lngSize = colHeader.Get("Content-Length")  '取得要下载文件的大小

        stRespones = hwp.GetResponseStream '取得响应流
        st = New FileStream(Application.StartupPath & "\MinecraftServer\" & dwlfilename, FileMode.Create) '本地保存文件

        intCurSize = stRespones.Read(bytBuffer, 0, bytBuffer.Length) '响应流中读取

        Do While (intCurSize > 0) '只要有数据就继续
            st.Write(bytBuffer, 0, intCurSize)     '写入本地文件
            intDiff = DateDiff(DateInterval.Second, datLast, Now)

            lngCurSize = lngCurSize + intCurSize
            lngNet = lngNet + intCurSize              '单位时间内的下载量
            If intDiff >= 1 Then
                Label3.Text = "文件大小：" & Math.Round(lngSize / 1024 / 1024, 2) & " MB" & vbCrLf &
                                "已经下载：" & Math.Round(lngCurSize / 1024 / 1024, 2) & " MB" & vbCrLf &
                                "下载速度：" & CInt(lngNet / intDiff / 1024 / 1024) & " MB/s" & vbCrLf &
                                "下载百分比：" & Math.Round(lngCurSize / lngSize * 100, 2) & " %"
                datLast = Now
                lngNet = 0
            End If

            Application.DoEvents()
            intCurSize = stRespones.Read(bytBuffer, 0, bytBuffer.Length) '继续读取
        Loop
        Label3.Text = "下载完成！"
        ToolStripStatusLabel1.Text = "下载完成！"
        TextBox2.Text = Application.StartupPath & "\MinecraftServer\" & dwlfilename

        st.Close()
        stRespones.Close()
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False

        '检测服务端目录
        If System.IO.Directory.Exists(Application.StartupPath & "/MinecraftServer") = False Then
            System.IO.Directory.CreateDirectory(Application.StartupPath & "/MinecraftServer")
        End If
    End Sub


    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        If (ComboBox2.Text = "Spigot") Then
            ListBox1.Items.Clear()
            ListBox1.Items.Add("双击下载")
            ListBox1.Items.Add("Spigot-1.16")
            'ListBox1.Items.Add("Spigot-1.15")
            'ListBox1.Items.Add("Spigot-1.14")
            'ListBox1.Items.Add("Spigot-1.13")
            'ListBox1.Items.Add("Spigot-1.12")
            'ListBox1.Items.Add("Spigot-1.11")
            ' ListBox1.Items.Add("Spigot-1.11")
            'ListBox1.Items.Add("Spigot-1.10")
            'ListBox1.Items.Add("Spigot-1.9")
            'ListBox1.Items.Add("Spigot-1.8")
        End If
    End Sub
    Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick
        If ListBox1.SelectedItem.ToString = "Spigot-1.16" Then
            dwlurl = "https://drive.dnxshare.xyz/2:/Spigot/spigot-1.16.5.jar"
            dwlfilename = "Spigot-1.16.jar"
            Label3.Text = "正在向下载源请求回源···"
            Button1.Enabled = False
            Dim xzq As Thread
            xzq = New Thread(AddressOf xzq1)
            xzq.Start()
        End If
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged
        serverurl = TextBox2.Text
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Try
            _process.StandardInput.WriteLine(TextBox3.Text)
            TextBox3.Text = ""
        Catch ex As Exception
            MsgBox(ex.Message, 16, "指令发送错误")
        End Try

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        _process.StandardInput.WriteLine("stop")
    End Sub

    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        If (ComboBox1.Text = "OP") Then
            TextBox3.Text = "op "
        End If
    End Sub

    Private Sub Process1_OutputDataReceived(sender As Object, e As DataReceivedEventArgs) Handles Process1.OutputDataReceived
        Dim handler As AddMessageHandler = Function(msg As String)
                                               '判断日志类型
                                               If InStr(1, msg, "INFO") <> 0 Then
                                                   TextBox1.AppendText("[信息]" & msg + Environment.NewLine)
                                               Else
                                                   If InStr(1, msg, "WARN") <> 0 Then
                                                       TextBox1.AppendText("[警告]" & msg + Environment.NewLine)
                                                   Else
                                                       If InStr(1, msg, "ERROR") <> 0 Then
                                                           TextBox1.AppendText("[错误]" & msg + Environment.NewLine)
                                                       Else
                                                           TextBox1.AppendText("[未知]" & msg + Environment.NewLine)
                                                       End If
                                                   End If
                                               End If
                                               '检测是否关闭
                                               If InStr(1, msg, "All chunks are saved") <> 0 Then
                                                   ToolStripStatusLabel1.Text = "服务端关闭！"
                                               End If
                                           End Function
        If Me.TextBox1.InvokeRequired Then
            Me.TextBox1.Invoke(handler, e.Data)
        End If
    End Sub

    Private Sub TextBox3_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox3.KeyDown
        If e.KeyCode = 13 Then
            Try
                TextBox3.Text = ""
                _process.StandardInput.WriteLine(TextBox3.Text)
            Catch ex As Exception
                MsgBox(ex.Message, 16, "指令发送错误")
            End Try
        End If
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs) Handles Me.Closed
        _process.StandardInput.WriteLine("stop")
    End Sub
End Class
