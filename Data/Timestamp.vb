Namespace Data
    <Serializable>
    Public Class Timestamp
        Inherits Record
        Sub New(Value As DateTime)
            Me.Value = Value
        End Sub
        Public Overrides ReadOnly Property Type As Type
            Get
                Return GetType(DateTime)
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return String.Format("{0}", Me.Value.ToString)
        End Function
    End Class
End Namespace