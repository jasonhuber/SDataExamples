<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLoader
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnLoadDataOLEDB = New System.Windows.Forms.Button
        Me.btnRun = New System.Windows.Forms.Button
        Me.txtFileLoc = New System.Windows.Forms.TextBox
        Me.txtOutput = New System.Windows.Forms.TextBox
        Me.opfd = New System.Windows.Forms.OpenFileDialog
        Me.lnkOpenFile = New System.Windows.Forms.LinkLabel
        Me.SuspendLayout()
        '
        'btnLoadDataOLEDB
        '
        Me.btnLoadDataOLEDB.Location = New System.Drawing.Point(354, 399)
        Me.btnLoadDataOLEDB.Name = "btnLoadDataOLEDB"
        Me.btnLoadDataOLEDB.Size = New System.Drawing.Size(173, 27)
        Me.btnLoadDataOLEDB.TabIndex = 8
        Me.btnLoadDataOLEDB.Text = "Import Lat & Lon (slxOLEDB.1)"
        Me.btnLoadDataOLEDB.UseVisualStyleBackColor = True
        '
        'btnRun
        '
        Me.btnRun.Location = New System.Drawing.Point(12, 399)
        Me.btnRun.Name = "btnRun"
        Me.btnRun.Size = New System.Drawing.Size(141, 27)
        Me.btnRun.TabIndex = 7
        Me.btnRun.Text = "Import Lat & Lon (sData)"
        Me.btnRun.UseVisualStyleBackColor = True
        '
        'txtFileLoc
        '
        Me.txtFileLoc.Location = New System.Drawing.Point(12, 373)
        Me.txtFileLoc.Name = "txtFileLoc"
        Me.txtFileLoc.Size = New System.Drawing.Size(515, 20)
        Me.txtFileLoc.TabIndex = 6
        Me.txtFileLoc.Text = "C:\Documents and Settings\Administrator\Desktop\classfiles\web\addresses.xml"
        '
        'txtOutput
        '
        Me.txtOutput.Location = New System.Drawing.Point(12, 12)
        Me.txtOutput.Multiline = True
        Me.txtOutput.Name = "txtOutput"
        Me.txtOutput.Size = New System.Drawing.Size(538, 355)
        Me.txtOutput.TabIndex = 5
        '
        'opfd
        '
        Me.opfd.Filter = "XML files (*.xml)|*.xml"
        Me.opfd.InitialDirectory = "C:\Documents and Settings\Administrator\Desktop\classfiles\"
        '
        'lnkOpenFile
        '
        Me.lnkOpenFile.AutoSize = True
        Me.lnkOpenFile.Location = New System.Drawing.Point(533, 380)
        Me.lnkOpenFile.Name = "lnkOpenFile"
        Me.lnkOpenFile.Size = New System.Drawing.Size(16, 13)
        Me.lnkOpenFile.TabIndex = 9
        Me.lnkOpenFile.TabStop = True
        Me.lnkOpenFile.Text = "..."
        '
        'frmLoader
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(567, 435)
        Me.Controls.Add(Me.lnkOpenFile)
        Me.Controls.Add(Me.btnLoadDataOLEDB)
        Me.Controls.Add(Me.btnRun)
        Me.Controls.Add(Me.txtFileLoc)
        Me.Controls.Add(Me.txtOutput)
        Me.Name = "frmLoader"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnLoadDataOLEDB As System.Windows.Forms.Button
    Friend WithEvents btnRun As System.Windows.Forms.Button
    Friend WithEvents txtFileLoc As System.Windows.Forms.TextBox
    Friend WithEvents txtOutput As System.Windows.Forms.TextBox
    Friend WithEvents opfd As System.Windows.Forms.OpenFileDialog
    Friend WithEvents lnkOpenFile As System.Windows.Forms.LinkLabel

End Class
