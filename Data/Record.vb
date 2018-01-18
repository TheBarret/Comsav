Namespace Data
    <Serializable>
    Public MustInherit Class Record
        Public Property Value As Object
        Public MustOverride ReadOnly Property Type As Type
        Public Function GetValue(Of T As Type)() As T
            Return CType(Me.Value, T)
        End Function
    End Class
End Namespace