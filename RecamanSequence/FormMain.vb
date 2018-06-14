Imports System.Threading

Public Class FormMain
    Private bmp As Bitmap
    Private g As Graphics

    Private genSequencethread As Thread
    Private refreshthread As Thread

    Private dst As Integer
    Private d As Integer
    Private x As Integer
    Private y As Integer
    Private f As Boolean = True

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, False)
        Me.SetStyle(ControlStyles.UserPaint, True)

        bmp = New Bitmap(Me.DisplayRectangle.Width, Me.DisplayRectangle.Height)
        g = Graphics.FromImage(bmp)
        g.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality
        g.InterpolationMode = Drawing2D.InterpolationMode.NearestNeighbor
        g.Clear(Color.Black)
        y = bmp.Height / 2
        dst = 8

        Using p As New Pen(Color.FromArgb(128, Color.DimGray))
            For xd As Integer = 0 To bmp.Width - 1 Step dst
                g.DrawLine(p, xd, 0, xd, bmp.Height)
            Next
        End Using

        genSequencethread = New Thread(Sub()
                                           Thread.Sleep(1000)

                                           Dim n As Integer = 0
                                           Dim v As Integer = 1
                                           Dim t As Integer
                                           Dim lt As Integer = 0
                                           Dim values As New List(Of Integer) From {n}

                                           Do
                                               t = n - v
                                               If t <= 0 OrElse values.Contains(t) Then
                                                   t = n + v
                                                   n = t
                                               End If

                                               values.Add(t)
                                               n = t
                                               v += 1

                                               d = dst * Math.Abs(t - lt)
                                               If Math.Sign(t - lt) = -1 Then lt = t
                                               If f Then
                                                   g.DrawArc(Pens.White, dst * lt, y - d \ 2, d, d, 180, 180)
                                               Else
                                                   g.DrawArc(Pens.White, dst * lt, y - d \ 2, d, d, -180, -180)
                                               End If
                                               f = Not f
                                               lt = t

                                               If values.Count > 1000000 Then Exit Do

                                               Thread.Sleep(25) ' Add some animation...
                                           Loop
                                       End Sub) With {.IsBackground = True
                                }
        genSequencethread.Start()

        refreshthread = New Thread(Sub()
                                       Do
                                           Thread.Sleep(30)
                                           Me.Invalidate()
                                       Loop
                                   End Sub) With {.IsBackground = True}
        refreshthread.Start()
    End Sub

    Private Sub FormMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        For x As Integer = 0 To Me.DisplayRectangle.Width - 1 Step dst
            e.Graphics.DrawLine(Pens.Gray, x, 0, x, Me.DisplayRectangle.Height)
        Next

        e.Graphics.DrawImageUnscaled(bmp, 0, 0)
    End Sub
End Class
