Namespace Data
    <Serializable>
    Public Class Unkown
        Inherits Record
        Sub New(Value As Object)
            Me.Value = Value
        End Sub
        Public Overrides ReadOnly Property Type As Type
            Get
                Return GetType(Object)
            End Get
        End Property
    End Class
End Namespace