using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

public partial class PdfMover : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (FileUpload1.HasFile)
        {
            try
            {
                // Generate a unique file name to avoid overwriting
                string fileName = Path.GetFileName(FileUpload1.FileName);
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                // Define the path to save the file
                //string savePath = Server.MapPath("~/UploadedFiles/") + uniqueFileName;
                string savePath = @"D:\Pdf\" + uniqueFileName;

                // Ensure the directory exists
                if (!Directory.Exists(@"D:\Pdf"))
                {
                    Directory.CreateDirectory(@"D:\Pdf");
                }
                // Save the file to the specified path
                FileUpload1.SaveAs(savePath);

                // Save the file information to the database
                SaveFileToDatabase(fileName, savePath);

                Label1.Text = "Upload status: File uploaded successfully!";
            }
            catch (Exception ex)
            {
                Label1.Text = "Upload status: The file could not be uploaded. The following error occurred: " + ex.Message;
            }
        }
        else
        {
            Label1.Text = "Upload status: No file selected.";
        }
    }

    private void SaveFileToDatabase(string fileName, string filePath)
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DbConnection"].ConnectionString;
        using (SqlConnection conn = new SqlConnection(connectionString))
        {
            string query = "INSERT INTO PdfFiles (FileName, FilePath) VALUES (@FileName, @FilePath)";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FileName", fileName);
                cmd.Parameters.AddWithValue("@FilePath", filePath);

                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }
    }
    protected void Button2_Click(object sender, EventArgs e)
    {
        string sourceDirectory = @"D:\Pdf";
        string destinationDirectory = @"D:\Download PDF";

        try
        {
            // Ensure the destination directory exists
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // Get all PDF files in the source directory
            string[] pdfFiles = Directory.GetFiles(sourceDirectory, "*.pdf");

            foreach (string pdfFile in pdfFiles)
            {
                // Get the file name
                string fileName = Path.GetFileName(pdfFile);

                // Define the destination path
                string destFile = Path.Combine(destinationDirectory, fileName);

                // Move the file to the destination directory
                File.Move(pdfFile, destFile);
            }

            StatusLabel.Text = "Files moved successfully!";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = "An error occurred: " + ex.Message;
        }
    }
    protected void Button3_Click(object sender, EventArgs e)
    {
        string sourceDirectory = @"D:\Pdf";
        string targetDirectory = @"D:\Download PDF";
        string mergedFileName = "MergedFile.pdf";
        string mergedFilePath = Path.Combine(sourceDirectory, mergedFileName);
        string targetFilePath = Path.Combine(targetDirectory, mergedFileName);

        try
        {
            // Merge PDFs
            MergePdfFiles(sourceDirectory, mergedFilePath);

            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            // Move the merged PDF to the target directory
            MoveFile(mergedFilePath, targetFilePath);

            // Provide the merged PDF for download
            //DownloadFile(targetFilePath);
            Label2.Text = "PDFs merged, moved, and downloaded successfully!";
        }
        catch (IOException ioEx)
        {
            // Handle file I/O exceptions
            Label2.Text = "File I/O error: " + ioEx.Message;
        }
        catch (UnauthorizedAccessException uaEx)
        {
            // Handle unauthorized access exceptions
            Label2.Text = "Unauthorized access error: " + uaEx.Message;
        }
        catch (Exception ex)
        {
            // Handle all other exceptions
            Label2.Text = "An unexpected error occurred: " + ex.Message;
        }
    }

    private void MergePdfFiles(string sourceDirectory, string outputFilePath)
    {
        string[] pdfFiles = Directory.GetFiles(sourceDirectory, "*.pdf");
        string tempFilePath = Path.Combine(sourceDirectory, "TempMergedFile.pdf");

        using (FileStream stream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            using (Document document = new Document())
            {
                PdfCopy pdf = new PdfCopy(document, stream);
                document.Open();
                foreach (string file in pdfFiles)
                {
                    using (PdfReader reader = new PdfReader(file))
                    {
                        pdf.AddDocument(reader);
                    }
                }
                document.Close();
            }
        }
        if (File.Exists(outputFilePath))
        {
            File.Delete(outputFilePath);
        }

        File.Move(tempFilePath, outputFilePath);
    }

    private void MoveFile(string sourceFilePath, string destinationFilePath)
    {
        // Ensure the target directory exists
        string destinationDirectory = Path.GetDirectoryName(destinationFilePath);
        if (!Directory.Exists(destinationDirectory))
        {
            Directory.CreateDirectory(destinationDirectory);
        }

        // Move the file
        if (File.Exists(sourceFilePath))
        {
            File.Move(sourceFilePath, destinationFilePath);
        }
    }
    private void DownloadFile(string filePath)
    {
        FileInfo file = new FileInfo(filePath);
        if (file.Exists)
        {
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment; filename=" + file.Name);
            Response.AddHeader("Content-Length", file.Length.ToString());
            Response.ContentType = "application/pdf";
            Response.WriteFile(file.FullName);
            Response.End();
        }
    }
}


