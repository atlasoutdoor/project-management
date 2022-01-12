<%@ Page Language="vb" AutoEventWireup="true" CodeFile="Edit.aspx.vb" Inherits="Project_Edit" %>
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
	<asp:UpdatePanel ID="UpdatePanel" runat="server" OnLoad="UpdatePanel_Load">
	<ContentTemplate>
		<table border="0" cellspacing="4" cellpadding="0">
			<tr>
				<td align="right" valign="top"></td>
				<td><h1>Edit Task</h1></td>
			</tr>
			<tr>
				<td></td>
				<td><asp:LinkButton ID="ButtonDelete" runat="server" OnClick="ButtonDelete_Click" Text="Delete Task" OnClientClick="return confirm('Delete this task?');" /></td>
			</tr>
			<tr>
				<td align="right" valign="top">Note:</td>
				<td><asp:TextBox ID="TextBoxNote" runat="server" Width="200"></asp:TextBox></td>
			</tr>
			<tr>
				<td align="right" valign="top">Resource:</td>
				<td><asp:DropDownList ID="DropDownListResource" runat="server"></asp:DropDownList></td>
			</tr>

			<tr>
				<td align="right" valign="top">Estimate:</td>
				<td><asp:DropDownList ID="DropDownListDuration" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownListDuration_SelectedIndexChanged"></asp:DropDownList></td>
			</tr>

			<tr>
				<td align="right" valign="top">Spent:</td>
				<td><asp:DropDownList ID="DropDownListSpent" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DropDownListSpent_SelectedIndexChanged"></asp:DropDownList></td>
			</tr>

			<tr>
				<td align="right" valign="top">Status:</td>
				<td><asp:RadioButtonList ID="RadioButtonListStatus" runat="server" OnSelectedIndexChanged="RadioButtonListStatus_SelectedIndexChanged" AutoPostBack="true">
				<asp:ListItem Value="planned"><div class="task_status planned"></div>Planned</asp:ListItem>
				<asp:ListItem Value="started"><div class="task_status started"></div>Started</asp:ListItem>
				<asp:ListItem Value="finished"><div class="task_status finished"></div>Finished</asp:ListItem>
				</asp:RadioButtonList></td>
			</tr>
			<asp:Panel runat="server" ID="PanelStartEnd">
			<tr>
				<td align="right" valign="top">Start:</td>
				<td><asp:Label ID="LabelStart" runat="server" /></td>
			</tr>            
			<tr>
				<td align="right" valign="top">End:</td>
				<td><asp:Label ID="LabelEnd" runat="server" /></td>
			</tr>            
			<tr>
				<td align="right" valign="top">Spent:</td>
				<td><asp:Label ID="LabelSpent" runat="server" /></td>
			</tr>            
			</asp:Panel>
					<tr>
				<td align="right"></td>
				<td>
					<asp:HiddenField ID="HiddenStart" runat="server" />
					<asp:HiddenField ID="HiddenEnd" runat="server" />
					<asp:HiddenField ID="HiddenSpent" runat="server" />

					<asp:Button ID="ButtonOK" runat="server" OnClick="ButtonOK_Click" Text="OK" />
					<asp:Button ID="ButtonCancel" runat="server" Text="Cancel" OnClick="ButtonCancel_Click" />
				</td>
			</tr>

		</table>
		</ContentTemplate>
		<Triggers>
		<asp:AsyncPostBackTrigger ControlID="RadioButtonListStatus" />
		<asp:PostBackTrigger ControlID="ButtonDelete" />
		<asp:PostBackTrigger ControlID="ButtonOK" />
		<asp:PostBackTrigger ControlID="ButtonCancel" />
		</Triggers>
		</asp:UpdatePanel>


		</div>

		<script type="text/javascript">
			document.getElementById("TextBoxNote").focus();
		</script>

		<script type="text/javascript">
			Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
				modal.stretch();
			});
		</script>
	</form>
</body>
</html>