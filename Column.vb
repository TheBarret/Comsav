Imports Comsav.Data

<Serializable>
Public Class Column
    Public Property Records As List(Of Record)
    Sub New()
        Me.Records = New List(Of Record)
    End Sub
    Public Overrides Function ToString() As String
        Return String.Format("{0} records", Me.Records.Count)
    End Function
End Class
