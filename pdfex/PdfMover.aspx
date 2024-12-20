<%@ Page Language="C#" AutoEventWireup="true" CodeFile="PdfMover.aspx.cs" Inherits="PdfMover" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
            <asp:Button ID="Button1" runat="server" Text="Button" OnClick="Button1_Click"/>
            <asp:Label ID="Label1" runat="server" Text="Label"></asp:Label>
        </div>
         <div>
            <asp:Button ID="Button2" runat="server" Text="Download PDFs" OnClick="Button2_Click" />
            <asp:Label ID="StatusLabel" runat="server" Text="" />
        </div>
        <div>
            <asp:Button ID="Button3" runat="server" Text="Merge PDFs and Download" OnClick="Button3_Click" />
            <asp:Label ID="Label2" runat="server" Text="" />
        </div>
    </form>
</body>
</html>
