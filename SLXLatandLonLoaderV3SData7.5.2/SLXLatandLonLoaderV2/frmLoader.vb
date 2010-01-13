Imports System.Data
Imports System.Data.OleDb
Imports System.Net
Imports System.IO
Imports System.Text
Imports System.Linq

Public Class frmLoader
    '  ReadOnly sdata As XNamespace = XNamespace.Get("http://schemas.sage.com/sdata/2007")
    ReadOnly cf As XNamespace = XNamespace.Get("http://www.microsoft.com/schemas/rss/core/2005")
    ReadOnly atom As XNamespace = XNamespace.Get("http://www.w3.org/2005/Atom")
    ReadOnly slx As XNamespace = XNamespace.Get("http://schemas.sage.com/dynamic/2007")
    ReadOnly sdata As XNamespace = XNamespace.Get("http://schemas.sage.com/sdata/2008/1")
    ReadOnly xhttp As XNamespace = XNamespace.Get("http://schemas.sage.com/sdata/http/2008/1")
    '            Dim result As String = client.DownloadString("http://localhost/sdata/slx/dynamic/addresses(" + myDataSet.Tables(0).Rows(i).Item(0) + ")")
    ReadOnly xsi As XNamespace = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance")

    Private client As Net.WebClient
    Private doc As XDocument
    Private address As XElement
    Private entry As XElement

 
    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click

      

         'I need a dataset to read the xml
        Dim myDataSet As New DataSet
       
        'a dataset can just read in well-formed xml
        myDataSet.ReadXml(txtFileLoc.Text)
        ' need a loop variable
        Dim i As Integer
        ' set up connection
        client = New WebClient()
        client.Encoding = Encoding.UTF8
        client.Headers.Add(HttpRequestHeader.ContentType, "application/atom+xml")
        client.Credentials = New NetworkCredential("Admin", [String].Empty)
        Dim etag As String
        For i = 0 To myDataSet.Tables(0).Rows.Count - 1
            'Dim result As String = client.DownloadString("http://localhost/sdata/slx/dynamic/addresses(" + myDataSet.Tables(0).Rows(i).Item(0) + ")")
            Dim result As String = client.DownloadString("http://localhost:3333/sdata/slx/dynamic/-/addresses('" + myDataSet.Tables(0).Rows(i).Item(0) + "')")
            ' remove unicode preamble - not most appropriate way to do this,but will do for now.
            'If result(0) = CChar("65279") Then
            result = result.Substring(1)
            ' End If
            doc = XDocument.Load(New StringReader(result))

            entry = (From Entry In doc.Descendants(atom + "entry") Select Entry).First()
            etag = entry.Element(xhttp + "etag").Value
            address = (From Entry In doc.Descendants(slx + "Address") Select Entry).First()

            If Not address Is Nothing Then
                'MessageBox.Show(myDataSet.Tables(0).Rows(i).Item(1))
                'check AA to ensure LAT and LON arent Lat and Lon or lat and lon
                address.Element(slx + "Lat").SetValue(myDataSet.Tables(0).Rows(i).Item(1))
                'we need to remove the xsi attribute from the lat and lon or they will not be updated
                address.Element(slx + "Lat").Attribute(xsi + "nil").Remove()
                address.Element(slx + "Lon").SetValue(myDataSet.Tables(0).Rows(i).Item(2))
                address.Element(slx + "Lon").Attribute(xsi + "nil").Remove()
                Dim putURI As Object = address.Attribute(sdata + "uri").Value

                Dim putPayload As String = "<entry><payload>" & address.ToString() & "</payload></entry>"
                client.Headers.Clear()

                'entry means update, so you would have an id and a valid
                client.Headers.Add(HttpRequestHeader.ContentType, "application/atom+xml;type=entry")
                client.Headers.Add(HttpRequestHeader.IfMatch, etag)

                client.UploadString(putURI, "PUT", putPayload)
                txtOutput.Text &= "Updated: " & myDataSet.Tables(0).Rows(i).Item(0) & " with " & _
               myDataSet.Tables(0).Rows(i).Item(1) & ", " & _
              myDataSet.Tables(0).Rows(i).Item(2) & vbNewLine
            End If
        Next
        myDataSet.Dispose()



        ''This is the code for the sData one.
        ''I need a dataset to read the xml
        'Dim myDataSet As New DataSet

        ''a dataset can just read in well-formed xml
        'myDataSet.ReadXml(txtFileLoc.Text)

        '' need a loop variable
        'Dim i As Integer

        '' set up connection
        'Dim client = New WebClient()

        'client.Encoding = Encoding.UTF8
        'client.Headers.Add(HttpRequestHeader.ContentType, "application/atom+xml")
        'client.Credentials = New NetworkCredential("Admin", [String].Empty)

        'For i = 0 To myDataSet.Tables(0).Rows.Count - 1

        '    Dim result As String = client.DownloadString("http://localhost:3333/sdata/slx/dynamic/addresses(" + myDataSet.Tables(0).Rows(i).Item(0) + ")")

        '    ' remove unicode preamble - not most appropriate way to do this, but will do for now.
        '    'If result(0) = CChar("65279") Then
        '    result = result.Substring(1)
        '    ' End If

        '    doc = XDocument.Load(New StringReader(result))
        '    address = (From entry In doc.Descendants(atom + "entry") Select entry).First()

        '    If Not address Is Nothing Then
        '        'check AA to ensure LAT and LON arent Lat and Lon or lat and lon
        '        address.Element(atom + "LAT").SetValue(myDataSet.Tables(0).Rows(i).Item(1))
        '        address.Element(atom + "LON").SetValue(myDataSet.Tables(0).Rows(i).Item(2))
        '        Dim putURI As Object = address.Element(atom + "id").Value
        '        Dim putPayload As String = address.ToString()
        '        client.Headers.Clear()

        '        'entry means update, so you would have an id and a valid  address object being passed
        '        'you can check http://localhost/sdata/$system/adapters for more(Information)
        '        client.Headers.Add(HttpRequestHeader.ContentType, "application/atom+xml;type=entry")
        '        client.UploadString(putURI, "PUT", putPayload)
        '        txtOutput.Text &= "Updated: " & _
        '            myDataSet.Tables(0).Rows(i).Item(0) & " with " & _
        '            myDataSet.Tables(0).Rows(i).Item(1) & ", " & _
        '            myDataSet.Tables(0).Rows(i).Item(2) & vbNewLine
        '    End If
        'Next
        'myDataSet.Dispose()

    End Sub

    Private Sub lnkOpenFile_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles lnkOpenFile.LinkClicked
        opfd.ShowDialog()
        txtFileLoc.Text = opfd.FileName
    End Sub

    Private Sub btnLoadDataOLEDB_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadDataOLEDB.Click
        'I need a dataset to read the xml
        Dim myDataSet As New DataSet
        'a dataset can just read in well-formed xml
        myDataSet.ReadXml(txtFileLoc.Text)

        ' need a loop variable
        Dim i As Integer

        'I will need the usual ADO components
        Const _ConnectionString As String = "Provider=SLXOLEDB.1;Password=;Persist Security Info=True;" & _
        "User ID=admin;Initial Catalog=Saleslogix_Eval;Data Source=windows2003base;" & _
        "Extended Properties=PORT=1706;LOG=ON"
        Const strSQL As String = "update address set lat = ?, lon = ? where addressid = ?"

        Dim myConnection As New OleDbConnection(_ConnectionString)
        myConnection.Open()
        Dim myCommand As New OleDbCommand(strSQL, myConnection)
        For i = 0 To myDataSet.Tables(0).Rows.Count - 1
            myCommand.Parameters.AddWithValue("Lat", myDataSet.Tables(0).Rows(i).Item(1))
            myCommand.Parameters.AddWithValue("Lon", myDataSet.Tables(0).Rows(i).Item(2))
            myCommand.Parameters.AddWithValue("AddressID", myDataSet.Tables(0).Rows(i).Item(0))
            txtOutput.Text &= "About to update " & myDataSet.Tables(0).Rows(i).Item(0) & " with " & myDataSet.Tables(0).Rows(i).Item(1) & ", " & myDataSet.Tables(0).Rows(i).Item(2) & vbNewLine

            myCommand.ExecuteNonQuery()

            myCommand.Parameters.Clear()
        Next
        myDataSet.Dispose()

        myCommand.Dispose()
        myConnection.Close()
        myConnection.Dispose()
    End Sub
End Class
