Namespace Data
    <Serializable>
    Public Class Number
        Inherits Record
        Sub New(Value As Double)
            Me.Value = Value
        End Sub
        Public Overrides ReadOnly Property Type As Type
            Get
                Return GetType(Double)
            End Get
        End Property
        Public Overrides Function ToString() As String
            Return String.Format("{0}", Me.Value.ToString)
        End Function
    End Class
End Namespace