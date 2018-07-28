Imports System.Drawing.Drawing2D
Imports System.Threading

Public Class FormMain
    Private genSequencethread As Thread
    Private refreshThread As Thread

    Private graphColor As Pen = Pens.White
    Private path As New GraphicsPath()

    Private Const dst As Integer = 8

    Private midPoint As Integer

    Private Sub FormMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SetStyle(ControlStyles.AllPaintingInWmPaint, True)
        Me.SetStyle(ControlStyles.OptimizedDoubleBuffer, True)
        Me.SetStyle(ControlStyles.ResizeRedraw, False)
        Me.SetStyle(ControlStyles.UserPaint, True)

        genSequencethread = New Thread(Sub()
                                           Thread.Sleep(1000)

                                           Dim v As Integer = 1
                                           Dim t As Integer
                                           Dim lt As Integer = 0
                                           Dim d As Integer
                                           Dim f As Boolean = True
                                           Dim values As New List(Of Integer) From {lt}
                                           midPoint = Me.DisplayRectangle.Height / 2

                                           Do
                                               t = lt - v
                                               If t <= 0 OrElse values.Contains(t) Then t = lt + v

                                               values.Add(t)
                                               v += 1

                                               d = dst * Math.Abs(t - lt)
                                               If Math.Sign(t - lt) = -1 Then lt = t
                                               SyncLock path
                                                   path.StartFigure()
                                                   If f Then
                                                       path.AddArc(dst * lt, midPoint - d \ 2, d, d, -180, -180)
                                                   Else
                                                       path.AddArc(dst * lt, midPoint - d \ 2, d, d, 180, 180)
                                                   End If
                                               End SyncLock
                                               f = Not f
                                               lt = t

                                               Thread.Sleep(25) ' Add some animation...
                                           Loop
                                       End Sub) With {.IsBackground = True
                                }
        genSequencethread.Start()

        refreshThread = New Thread(Sub()
                                       Do
                                           Thread.Sleep(30)
                                           Me.Invalidate()
                                       Loop
                                   End Sub) With {.IsBackground = True}
        refreshThread.Start()
    End Sub

    Private Sub FormMain_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        Dim g As Graphics = e.Graphics

        g.Clear(Color.Black)
        SyncLock path
            Dim r As RectangleF = path.GetBounds()
            If r.Width > 0 AndAlso r.Height > 0 Then
                ' Math.Min adjust the graph so that its width fits in the window
                ' Math.Max adjust the graph so that its height fits in the window
                Dim a As Double = Math.Max(Me.DisplayRectangle.Width / r.Width, Me.DisplayRectangle.Height / r.Height)

                g.ScaleTransform(a, a)
                g.TranslateTransform(0, 0.5 * Me.DisplayRectangle.Height / a - midPoint)

                If a >= 0.8 AndAlso dst > 1 Then
                    r = g.ClipBounds
                    Using p As New Pen(Color.FromArgb(Math.Min(a / 6, 1) * 255, Color.Gray))
                        For x As Integer = r.Left To r.Right - 1 Step dst
                            g.DrawLine(p, x, r.Top, x, r.Bottom)
                        Next
                    End Using
                End If

                g.DrawPath(graphColor, path)
            End If
        End SyncLock
    End Sub
End Class
