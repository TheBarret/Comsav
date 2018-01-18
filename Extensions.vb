Module Extensions
    <System.Runtime.CompilerServices.Extension>
    Public Function GetString(Style As DocumentStyle) As String
        Select Case Style
            Case DocumentStyle.Windows
                Return ControlChars.CrLf
            Case DocumentStyle.Mac
                Return ControlChars.Cr
            Case DocumentStyle.Unix
                Return ControlChars.Lf
            Case Else
                Return String.Empty
        End Select
    End Function
End Module
