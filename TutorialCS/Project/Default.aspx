<%@ Page Language="C#" MasterPageFile="Project.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Project_Default" Title="Project"  %>
<%@ Register Assembly="DayPilot" Namespace="DayPilot.Web.Ui" TagPrefix="DayPilot" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" Runat="Server">
<script src='<%# ResolveUrl("~/Scripts/DayPilot/modal.js") %>' type="text/javascript"></script>
<script src='<%# ResolveUrl("~/Scripts/DayPilot/common.js") %>' type="text/javascript"></script>
<script src='<%# ResolveUrl("~/Scripts/App/gantt.js") %>' type="text/javascript"></script>

<asp:ScriptManager runat="server"></asp:ScriptManager>

<div class="space">
<a href="javascript:create('<%# DateTime.Today.ToString("s") %>')" class="button">New task</a>
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

<div style="overflow-x:auto; width: 100%; position: relative;" id="dps">
<DayPilot:DayPilotScheduler ID="DayPilotScheduler1" runat="server" 
        HeaderFontSize="8pt" 
        HeaderHeight="20" 
        DataStartField="Start" 
        DataEndField="End" 
        DataTextField="Text" 
        DataValueField="Id" 
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
        OnHeaderColumnWidthChanged="DayPilotScheduler1_HeaderColumnWidthChanged"
        OnBeforeResHeaderRender="DayPilotScheduler1_BeforeResHeaderRender"
        

        OnBeforeEventRender="DayPilotScheduler1_BeforeEventRender"
        ViewType="Gantt"
        >
        <HeaderColumns>
        <DayPilot:RowHeaderColumn title="Task" Width="150" />
        <DayPilot:RowHeaderColumn title="Estimate" Width="80" />
        <DayPilot:RowHeaderColumn title="Spent" Width="60" />
        <DayPilot:RowHeaderColumn title="Resource" Width="60" />
        </HeaderColumns>
    </DayPilot:DayPilotScheduler>
</div>

<div>
<asp:Label runat="server" ID="LabelSummary" />
</div>

<div style="display:none">
<asp:HiddenField ID="HiddenOrder" runat="server" />
<asp:Button id="ButtonReorder" runat="server" OnClick="ButtonReorder_Click" />
<asp:Button id="ButtonRefresh" runat="server" OnClick="ButtonRefresh_Click" />
</div>
</ContentTemplate>
</asp:UpdatePanel>

<script type="text/javascript">
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function (sender, args) {
        init();
    });


    var id = {};
    id.refreshButton = '<%# ButtonRefresh.UniqueID %>';
    id.reorderButton = '<%# ButtonReorder.UniqueID %>';
    id.project = <%# ProjectId %>;
    id.root = '<%# ResolveUrl("~/") %>';

    var drag = {};
    function init() {
        $(document).ready(function () {
            $(".task_status.planned")
            .css("cursor", "move")
            .attr('unselectable', 'on')
            .css('user-select', 'none')
            .css('-webkit-user-select', 'none')
            .css('-moz-user-select', 'none')
            .each(function () {
                this.onselectstart = function (e) { if (e && e.preventDefault) e.preventDefault(); return false; };
            });
            $(".task_status.planned").mousedown(function (e) {
                var id = $(this).data("taskid");
                drag.active = true;
                drag.id = id;
                drag.start = DayPilot.mo3($("#dps")[0], e.originalEvent);
                drag.source = $(this);

                var div = document.createElement("div");
                div.style.position = "absolute";
                div.style.height = "2px";
                div.style.width = "60px";
                div.style.backgroundColor = "red";
                div.style.display = "none";
                div.style.userSelect = "none";
                div.style.webkitUserSelect = "none";
                div.style.MozUserSelect = "none";
                div.setAttribute("unselectable", "on");
                $("#dps")[0].appendChild(div);
                $(document.body).addClass("moving");

                drag.div = div;
            });
            $(document).mousemove(function (e) {
                if (!drag.active) {
                    return;
                }
                drag.source.parent().css({ opacity: 0.5 });

                var rowHeight = 20;
                var headerHeight = 20;
                var coords = DayPilot.mo3($("#dps")[0], e.originalEvent);
                drag.position = Math.floor((coords.y - headerHeight) / rowHeight);
                drag.position = Math.max(0, drag.position);

                var offset = $("#dps").offset();
                offset.top = 0;
                offset.left = 0;
                drag.div.style.top = (offset.top + rowHeight * drag.position + headerHeight) + "px";
                drag.div.style.left = offset.left + "px";
                drag.div.style.display = "";
            });
            $(document).mouseup(function () {
                if (!drag.active) {
                    return;
                }

                if (drag.div && drag.div.parentNode) {
                    drag.div.parentNode.removeChild(drag.div);
                }
                $(document.body).removeClass("moving");

                var order = [];
                var placed = false;
                $(".task_status").each(function (index) {
                    if (!$(this).hasClass("planned")) {
                        return;
                    }
                    var id = $(this).data("taskid");
                    //alert("id:" + id);

                    if (index == drag.position) {
                        order.push(drag.id);
                        placed = true;
                    }

                    if (id == drag.id) {
                        return;
                    }
                    order.push(id);
                });

                if (!placed) {
                    order.push(drag.id);
                }

                //alert("new order:" + order.join(","));
                updateOrder(order.join(','));

                drag = {};
            });
        });
    }

    function updateOrder(order) {
        $("#<%# HiddenOrder.ClientID %>").val(order);
        __doPostBack(id.reorderButton, '');
    }

    init();
</script>


</asp:Content>

