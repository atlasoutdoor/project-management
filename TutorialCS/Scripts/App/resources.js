function createGroup() {
    createModal().showUrl(id.root + 'Project/NewGroup.aspx/' + id.project);
}

function createResource() {
    createModal().showUrl(id.root + 'Project/NewResource.aspx/' + id.project);
}

function edit(id) {
    createModal().showUrl('Edit.aspx?id=' + id);
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
