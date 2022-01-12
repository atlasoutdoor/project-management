<%@ Page Language="vb" MasterPageFile="Project.master" AutoEventWireup="true" CodeFile="Properties.aspx.vb" Inherits="Project_Properties" Title="Project"  %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src='<%#ResolveUrl("~/Scripts/DayPilot/modal.js")%>' type="text/javascript"></script>
<script src='<%#ResolveUrl("~/Scripts/App/properties.js")%>' type="text/javascript"></script>

<asp:ScriptManager runat="server"></asp:ScriptManager>

<asp:UpdatePanel ID="UpdatePanel" runat="server">
<ContentTemplate>

<div class="space">
<a href="javascript:editName()" class="button">Edit</a>
</div>

<div>

<div class="space">
<div class="label">Name</div>
<div><asp:Label id="LabelName" runat="server"></asp:Label></div>
</div>

<div style="display:none">
<asp:Button id="ButtonRefresh" runat="server" OnClick="ButtonRefresh_Click" />
</div>
</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
	var id = {};
	id.refreshButton = '<%#ButtonRefresh.UniqueID%>';
	id.project = <%#ProjectId%>;
	id.root = '<%#ResolveUrl("~/")%>';
</script>

</asp:Content>