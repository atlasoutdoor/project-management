<%@ Page Language="C#" MasterPageFile="Project.master" AutoEventWireup="true" CodeFile="Resources.aspx.cs" Inherits="Project_Resources" Title="Project"  %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src='<%# ResolveUrl("~/Scripts/DayPilot/modal.js") %>' type="text/javascript"></script>
<script src='<%# ResolveUrl("~/Scripts/App/resources.js") %>' type="text/javascript"></script>

<asp:ScriptManager runat="server"></asp:ScriptManager>

<div class="space">
<a href="javascript:createGroup()" class="button">New Group</a>
<a href="javascript:createResource()" class="button">New Resource</a>
</div>

<asp:UpdatePanel ID="UpdatePanelScheduler" runat="server" OnLoad="UpdatePanelScheduler_Load">
<ContentTemplate>

<div>
Zoom Level: <asp:RadioButtonList runat="server" ID="RadioButtonListZoom" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="RadioButtonListZoom_SelectedIndexChanged">
<asp:ListItem Selected="True">Hours</asp:ListItem>
<asp:ListItem>Days</asp:ListItem>
</asp:RadioButtonList>

<asp:CheckBox ID="CheckBoxHideFinished" runat="server" OnCheckedChanged="CheckBoxHideFinished_CheckedChanged" Text="Hide Finished Tasks" AutoPostBack="true" Checked="true" />
</div>


<div style="overflow-x:auto; width: 100%;">
<DayPilot:DayPilotScheduler ID="DayPilotScheduler1" runat="server" 
        HeaderFontSize="8pt" 
        HeaderHeight="20" 
        DataStartField="Start" 
        DataEndField="End" 
        DataTextField="Text" 
        DataValueField="Id" 
        DataResourceField="ResourceId" 
        EventHeight="20"
        EventFontSize="11px" 
        CellDuration="1440" 
        CellWidth="50"
        Days="31" 

        BorderColor="#aaaaaa"
        EventBorderColor="#aaaaaa"

        TimeRangeSelectedHandling="JavaScript"
        TimeRangeSelectedJavaScript="create('{0}', null, '{1}');"
        EventClickHandling="JavaScript"
        EventClickJavaScript="edit('{0}')"

        OnBeforeEventRender="DayPilotScheduler1_BeforeEventRender"
        RowHeaderColumnWidths="150"
        >
    </DayPilot:DayPilotScheduler>
</div>
<div style="display:none">
<asp:Button id="ButtonRefresh" runat="server" OnClick="ButtonRefresh_Click" />
</div>

</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">

    var id = {};
    id.refreshButton = '<%# ButtonRefresh.UniqueID %>';
    id.project = <%# ProjectId %>;
    id.root = '<%# ResolveUrl("~/") %>';

</script>
</asp:Content>

