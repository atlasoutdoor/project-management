<%@ Page Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Default" Title="Projects"  %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src="Scripts/DayPilot/modal.js" type="text/javascript"></script>
<script src="Scripts/App/projects.js" type="text/javascript"></script>

<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
<asp:UpdatePanel ID="UpdatePanelProjects" runat="server" OnLoad="UpdatePanelProjects_Load">
<ContentTemplate>

<div class="space">
<a href="javascript:create()" class="button">New Project</a>
</div>

<asp:Repeater ID="RepeaterProjects" runat="server">
<ItemTemplate>
<div><a href='Project/Default.aspx/<%# Eval("ProjectId") %>'><%# Eval("ProjectName") %></a></div>
</ItemTemplate>
</asp:Repeater>

<div style="display:none">
<asp:Button id="ButtonRefresh" runat="server" OnClick="ButtonRefresh_Click" />
</div>


</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    var id = {};
    id.refreshButton = '<%# ButtonRefresh.UniqueID %>';
</script>

</asp:Content>

