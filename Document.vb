Imports System.IO
Imports System.Text
Imports System.Globalization
Imports Comsav.Data

<Serializable>
Public Class Document
    Inherits Dictionary(Of String, Column)
    Implements IDisposable
    Public Const Delimiter As Char = ","c
    Public Property Encoder As Encoding
    Public Property Culture As CultureInfo
    Public Property Style As DocumentStyle
    Sub New()
        Me.Encoder = Encoding.UTF8
        Me.Culture = New CultureInfo("en-US")
    End Sub
    Sub New(Encoder As Encoding)
        Me.Encoder = Encoder
        Me.Culture = New CultureInfo("en-US")
    End Sub
    Sub New(Encoder As Encoding, Culture As CultureInfo)
        Me.Encoder = Encoder
        Me.Culture = Culture
    End Sub
    Private m_filename As String
    Public ReadOnly Property Filename As String
        Get
            Return Me.m_filename
        End Get
    End Property
    Public Function GetHeaders() As String()
        Return Me.Select(Function(x) x.Key).ToArray
    End Function
    Public Function GetValuesAtColumn(Of T)(Index As Integer) As T()
        If (Index >= 0 AndAlso Index <= Me.Count - 1) Then
            Return Me.ElementAt(Index).Value.Records.Where(Function(x) x.Type = GetType(T)).Select(Function(y) CType(y.Value, T)).ToArray
        End If
        Throw New Exception(String.Format("index '{0}' is out of range", Index))
    End Function
    Public Function GetValuesAtColumn(Of T)(Name As String) As T()
        If (Me.ContainsKey(Name.ToLower)) Then
            Return Me(Name.ToLower).Records.Where(Function(x) x.Type = GetType(T)).Select(Function(y) CType(y.Value, T)).ToArray
        End If
        Throw New Exception(String.Format("column '{0}' does not exist", Name))
    End Function
    Public Shared Function Parse(Filename As String) As Document
        Dim doc As New Document With {.m_filename = Filename}
        If (File.Exists(Filename)) Then
            Dim values As List(Of String)
            Using sr As StreamReader = Document.Read(Filename, doc.Style, doc.Encoder)
                For Each line As String In Document.GetLines(sr, doc)
                    If (Document.IsHeader(line)) Then
                        For Each pair As KeyValuePair(Of String, Column) In Document.GetHeaders(line)
                            doc.Add(pair.Key.ToLower, pair.Value)
                        Next
                    Else
                        values = Document.GetValues(line)
                        For i As Integer = 0 To values.Count - 1
                            If (doc.Count < i + 1) Then
                                doc.Add((String.Format("column-{0}", i + 1)), New Column)
                            End If
                            If (Document.IsDate(values(i))) Then
                                doc.ElementAt(i).Value.Records.Add(New Timestamp(Document.GetDate(values(i))))
                            ElseIf (Document.IsNumber(values(i), doc.Culture)) Then
                                doc.ElementAt(i).Value.Records.Add(New Number(Document.GetNumber(values(i), doc.Culture)))
                            Else
                                doc.ElementAt(i).Value.Records.Add(New Unkown(values(i)))
                            End If
                        Next
                    End If
                Next
            End Using
        End If
        Return doc
    End Function
    Private Shared Function GetLines(stream As StreamReader, Document As Document) As List(Of String)
        Dim buffer As New List(Of String)
        For Each value As String In Strings.Split(stream.ReadToEnd, Document.Style.GetString)
            If (value.Trim.Length > 0) Then
                buffer.Add(value.Trim)
            End If
        Next
        Return buffer
    End Function
    Private Shared Function GetHeaders(Line As String) As Dictionary(Of String, Column)
        Dim buffer As New Dictionary(Of String, Column)
        For Each name As String In Line.Split(Document.Delimiter)
            buffer.Add(name, New Column)
        Next
        Return buffer
    End Function
    Private Shared Function GetValues(Line As String) As List(Of String)
        Dim buffer As New List(Of String)
        For Each value As String In Line.Split(Document.Delimiter)
            buffer.Add(value)
        Next
        Return buffer
    End Function
    Private Shared Function GetNumber(value As String, Culture As CultureInfo) As Double
        Return Double.Parse(value, NumberStyles.Float, Culture)
    End Function
    Private Shared Function GetDate(value As String) As DateTime
        Return DateTime.Parse(value)
    End Function
    Private Shared Function IsNumber(Value As String, Culture As CultureInfo) As Boolean
        Return Double.TryParse(Value, NumberStyles.Float, Culture, Nothing)
    End Function
    Private Shared Function IsDate(Value As String) As Boolean
        Return DateTime.TryParse(Value, Nothing)
    End Function
    Private Shared Function IsHeader(Line As String) As Boolean
        Return Not Line.Any(Function(ch) Char.IsDigit(ch))
    End Function
    Public Shared Function Read(Filename As String, ByRef Style As DocumentStyle, Encoder As Encoding) As StreamReader
        If (File.Exists(Filename)) Then
            Dim stream As New StreamReader(Filename, Encoder), buffer As String = stream.ReadToEnd
            Try
                For Each value As DocumentStyle In [Enum].GetValues(GetType(DocumentStyle))
                    Select Case value
                        Case DocumentStyle.Windows
                            If (buffer.IndexOf(ControlChars.CrLf) > 0) Then
                                Style = DocumentStyle.Windows
                                Exit For
                            End If
                        Case DocumentStyle.Mac
                            If (buffer.IndexOf(ControlChars.Cr) > 0) Then
                                Style = DocumentStyle.Mac
                                Exit For
                            End If
                        Case DocumentStyle.Unix
                            If (buffer.IndexOf(ControlChars.Lf) > 0) Then
                                Style = DocumentStyle.Unix
                                Exit For
                            End If
                        Case Else
                            Style = DocumentStyle.Unkown
                            Exit For
                    End Select
                Next
            Finally
                stream.BaseStream.Position = 0
            End Try
            Return stream
        End If
        Return Nothing
    End Function
#Region "IDisposable Support"
    Private disposedValue As Boolean
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                Me.Clear()
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Me.Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
