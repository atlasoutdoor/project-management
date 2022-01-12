<%@ Page Language="vb" AutoEventWireup="true" CodeFile="PropertiesEditName.aspx.vb" Inherits="Project_PropertiesEditName" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<link href="~/Media/project.css" rel="stylesheet" type="text/css" />
	<title>Edit</title>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
	<div>
	<asp:UpdatePanel ID="UpdatePanel" runat="server">
	<ContentTemplate>
		<table border="0" cellspacing="4" cellpadding="0">
			<tr>
				<td align="right" valign="top"></td>
				<td><h1>Edit Project Name</h1></td>
			</tr>
			<tr>
				<td align="right" valign="top">Name:</td>
				<td><asp:TextBox ID="TextBoxName" runat="server" Width="200"></asp:TextBox></td>
			</tr>
					<tr>
				<td align="right"></td>
				<td>
					<asp:Button ID="ButtonOK" runat="server" OnClick="ButtonOK_Click" Text="OK" />
					<asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClick="ButtonCancel_Click" />
				</td>
			</tr>

		</table>
		</ContentTemplate>
		<Triggers>
		<asp:PostBackTrigger ControlID="ButtonOK" />
		<asp:PostBackTrigger ControlID="ButtonCancel" />
		</Triggers>
		</asp:UpdatePanel>


		</div>

		<script type="text/javascript">
			document.getElementById("TextBoxName").focus();
		</script>

		<script type="text/javascript">
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
				modal.stretch();
			});
		</script>
	</form>
</body>
</html>