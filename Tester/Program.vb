Imports Comsav

Module Program

    Sub Main()
        Using doc As Document = Document.Parse(".\Sample.csv")

            Dim a As Double() = doc.GetValuesAtColumn(Of Double)("Open")    '// Fetch values at column using name
            Dim b As DateTime() = doc.GetValuesAtColumn(Of DateTime)(0)     '// Fetch values at column using index

            Debugger.Break()
        End Using


    End Sub

End Module
