function editName() {
    createModal().setHeight(300).showUrl(id.root + 'Project/PropertiesEditName.aspx/' + id.project);
}

function createModal() {
    var modal = new DayPilot.Modal();
    modal.top = 60;
    modal.width = 300;
    modal.opacity = 50;
    modal.border = "10px solid #d0d0d0";
    modal.closed = function () {
        if (this.result && this.result.refresh) {
            __doPostBack(id.refreshButton, '');
        }
    };

    modal.setHeight = function (height) {
        modal.height = height;
        return modal;
    };

    modal.height = 260;
    modal.zIndex = 100;

    return modal;
}
