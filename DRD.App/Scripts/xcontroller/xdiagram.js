myApp.controller("xdiagramController", function ($scope, Upload, $location, $http, $filter) {

    // modal member
    $scope.member = {};
    $scope.members = [];
    $scope.memberCount = [];
    $scope.paging = [];
    $scope.criteria = "";
    $scope.page = 1;
    $scope.row = 20;
    $scope.currPage = 0;
    $scope.index = 0;
    // modal member

    $scope.nodeItem = { element: '', memberId: 0, symbolCode: '', caption: '', info: '', Operator: '', value: 0, textColor: '', backColor: '', posLeft: 0, posTop: 0, width: 0, height: 0, member: { number: '', name: '', email: '', imageProfile: '' } };
    $scope.nodes = [];
    $scope.linkItem = { elementFrom: '', elementTo: '', firstNode: '', endNode: '', symbolCode: '', caption: '', Operator: '', value: 0, line: {} };
    $scope.links = [];
    $scope.inode = 1;
    $scope.elmActivityName = "node-activity";
    $scope.elmTitleName = "title-activity";
    $scope.elmDecisionName = "node-decision";
    $scope.elmTransferName = "node-transfer";
    $scope.elmCaseName = "node-case";
    $scope.elmPararrelName = "node-pararrel";

    $scope.linkTypeSubmit = "grid";
    $scope.linkTypeRevisi = "magnet";
    $scope.linkTypeReject = "grid";
    $scope.linkTypeAlter = "magnet";
    $scope.linkTypeYes = "grid";
    $scope.linkTypeNo = "grid";
    $scope.linkTypeSubmitCase = "grid";
    $scope.linkEndPlug = 'arrow1';
    $scope.linkLabelFontSize = '8pt';

    $scope.linkColorSubmit = "black";
    $scope.linkColorReject = "red";
    $scope.linkColorRevisi = "chocolate";
    $scope.linkColorAlter = "dodgerblue";
    $scope.linkColorYes = "green";
    $scope.linkColorNo = "red";
    $scope.linkColorSubmitCase = "darkgrey";

    $scope.selectedLinkNodeNo = -1;
    $scope.selectedLinkNo = -1;
    $scope.selectedLinkNoIdx = -1;

    $scope.firstNode = '';
    $scope.endNode = '';

    $scope.activityNumber = 1;
    $scope.activity = {};
    $scope.decision = {};
    $scope.transfer = {};
    $scope.case = {};
    $scope.linkcase = { Value: 0, Operator: '=' };
    $scope.linkalter = { Period: 0, PeriodUnit: 'HOUR' };

    $scope.operators = [{ Id: 0, Descr: '<' }, { Id: 1, Descr: '>' }, { Id: 2, Descr: '=' }, { Id: 3, Descr: '<=' }, { Id: 4, Descr: '>=' }, { Id: 5, Descr: '<>' }, { Id: 6, Descr: 'ELSE' }];
    $scope.decOperators = [];
    $scope.periods = [{ Id: 0, Descr: 'HOUR' }, { Id: 1, Descr: 'DAY' }];
    $scope.decision.Operator = "";
    /*--------------------------------------------------------------
    INIT DATA
    --------------------------------------------------------------*/
    angular.element(document).ready(function () {
        $scope.initTerminator();
    });

    function initValues() {

    }

    $scope.initData = function () {

    }

    $scope.initTerminator = function () {



        $("#start-0").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });


        $scope.addNode('start-0', 'START', 'Start');

        $scope.initUpload();

        var from = document.getElementById("start-0");
        from.style.left = 50;
        var to = document.getElementById("node-activity-2");
        $scope.addLinkSubmit(from, to);
        $scope.refreshLines();

        $("#end-1").droppable({
            drop: function (event, ui) {
                var fromIdx = $scope.getIndex(ui.draggable.context.id);
                var from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                var to = document.getElementById("end-1");
                $scope.endNode = to;

                if (fromIdx!= 2 && ui.draggable.context.id == 'submit-' + fromIdx && fromIdx != 'start') {
                    $scope.addLinkSubmit(from, to);
                } else if (ui.draggable.context.id == 'reject-' + fromIdx) {
                    $scope.addLinkReject(from, to);
                }
            },

        });

        $("#end-1").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $scope.addNode('end-1', 'END', 'End');
        $scope.firstNode = document.getElementById("node-activity-2");

    }

    $scope.initUpload = function () {
        var activityId = "node-activity-2"
        var submitId = "submit-2";

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);

        $("#node-activity-2").show();
        var lineSubmit = new LeaderLine(activity, submit, {
                endPlug: 'behind',
                hide: true,
                path: 'magnet',
                dash: { animation: true },
                size: 2,
                color: $scope.linkColorSubmit,
            });

        $("#submit-2").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineSubmit.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineSubmit.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });

        $("#node-activity-2").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $scope.addNode('node-activity-2', 'ACTIVITY', 'activity');
    }

    $scope.initActivity = function (idNo) {
        var activityId = $scope.elmActivityName + "-" + idNo;
        var submitId = "submit-" + idNo;

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);


        var
            lineSubmit = new LeaderLine(activity, submit, {
                endPlug: 'behind',
                hide: true,
                path: 'magnet',
                dash: { animation: true },
                size: 2,
                color: $scope.linkColorSubmit,
            });

        $("#" + submitId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineSubmit.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineSubmit.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });

        $("#" + activityId).droppable({
            drop: function (event, ui) {
                var fromIdx = $scope.getIndex(ui.draggable.context.id);
                var from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                var to = document.getElementById($scope.elmActivityName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-2') {
                    from = document.getElementById('node-activity-2');
                    $scope.addLinkSubmit(from, to);
                }else if (ui.draggable.context.id == 'submit-' + fromIdx) {
                    $scope.addLinkSubmit(from, to);
                }
               
            },

        });

        $("#" + activityId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });


    }

    $scope.refreshLines = function () {
        for (i = 0; i < $scope.links.length; i++) {
            $scope.links[i].line.position().show();
        }
    }

    $scope.refreshLine = function (idx) {
        $scope.links[idx].line.position().show();
    }

    $scope.addNodeActivity = function (left, top) {
        var canvas = document.getElementById('canvas');

        var activityName = 'Activity ' + $scope.activityNumber
        $scope.activityNumber += 1;

        console.log($scope.activityNumber);
        console.log(activityName);

        var html = document.getElementById($scope.elmActivityName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        html = html.replace(/ActivTitle/g, activityName );
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmActivityName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initActivity($scope.inode);

        $scope.addNode($scope.elmActivityName + '-' + $scope.inode, 'ACTIVITY', activityName);

        var idx = $scope.inode - 1;
        let elmTitle = 'title-activity-' + idx;

        $(document).on("dblclick", "#" + elmTitle, function () {

            var current = $(this).text();
            console.log(current);
            $("#" + elmTitle).html('<input class="form-control" id="newcont" rows="5"/>');
            $("#newcont").val(current);
            $("#newcont").focus();

            $("#newcont").focus(function () {
                console.log('in');
            }).blur(function () {
                var newcont = $("#newcont").val();
                if (newcont == "") {
                    alert("you should not leave this part empty");
                    newcont = current==""?"Activity":current;
                }
                $("#" + elmTitle).text(newcont);
            }).keyup(function (e) {
                if (e.keyCode == 13) {
                    var newcont = $("#newcont").val();
                    if (newcont == "") {
                        alert("you should not leave this part empty");
                        newcont = current==""?"Activity":current;
                    }
                    $("#" + elmTitle).text(newcont);
                }
            });

        })

        // Popover
        $('[data-popup="popover"]').popover();
        // Tooltip
        $('[data-popup="tooltip"]').tooltip();

        return newDiv;
    }

    $scope.addNode = function (elm, sym, cap) {
        var node = angular.copy($scope.nodeItem);
        node.element = elm;
        node.symbolCode = sym;
        node.caption = cap;
        $scope.nodes.push(node);

        $scope.inode++;
    }

    $scope.addLink = function (elmFrom, elmTo, frstNode, endElement, sym, cap, line) {

        var isready = false;
        for (i = 0; i < $scope.links.length; i++) {
            var lnk = $scope.links[i];
            if ((lnk.elementFrom == elmFrom && lnk.elementTo == elmTo && lnk.symbolCode == sym) ||
                (lnk.elementFrom == elmFrom && sym == 'ALTER') ||
                (lnk.elementFrom == elmFrom && elmFrom.includes('transfer') && sym == 'SUBMIT')) {
                isready = true;
                break;
            }
        }

        var link;
        if (!isready)
            link = angular.copy($scope.linkItem);
        else {
            link = $scope.links[i];
            if (link.line != undefined && link.line._id != undefined)
                link.line.remove();
        }

        link.firstNode = "node-activity-2";
        link.endNode = "end-1";
        link.elementFrom = elmFrom;
        link.elementTo = elmTo;
        link.symbolCode = sym;
        link.caption = cap;
        link.line = line;

        if (!isready)
            $scope.links.push(link);
    }

    $scope.addLinkSubmit = function (elmFrom, elmTo) {
        if (elmFrom.id == elmTo.id) return;

        var line = new LeaderLine(elmFrom, elmTo,
            {
                endPlug: $scope.linkEndPlug, path: $scope.linkTypeSubmit, size: 2, color: $scope.linkColorSubmit, fontSize: $scope.linkLabelFontSize
            });

        $scope.addLink(elmFrom.id, elmTo.id, $scope.firstNode.id, $scope.endNode.id, 'SUBMIT', '', line);
    }

    $scope.addLinkSubmit = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
            {
                endPlug: $scope.linkEndPlug, path: $scope.linkTypeSubmit, size: 2, color: $scope.linkColorSubmit, fontSize: $scope.linkLabelFontSize
            });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'SUBMIT', '', line);
    }

    $scope.removeActivity = function (idx) {
        $scope.removeNode($scope.elmActivityName + "-" + idx);
    }

    $scope.removeNode = function (elmName) {
        $("#" + elmName).remove();

        for (i = $scope.links.length - 1; i >= 0; i--) {
            if ($scope.links[i].elementFrom == elmName || $scope.links[i].elementTo == elmName) {
                $scope.links[i].line.remove();
                $scope.links.splice(i, 1);
            }
        }

        for (i = $scope.nodes.length - 1; i >= 0; i--) {
            if ($scope.nodes[i].element == elmName) {
                $scope.nodes.splice(i, 1);
            }
        }
        $scope.refreshLines();
    }

    $scope.removeActivityConfirmation = function (idx) {
        swal({
            title: "Confirmation",
            text: "This node will be deleted, are you sure?",
            type: "warning",
            showCancelButton: true,
            closeOnConfirm: false,
            confirmButtonColor: "#2196F3",
            showLoaderOnConfirm: true,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, cancel please!",
        },
            function (isConfirm) {
                //setTimeout(function () {
                if (isConfirm)
                    $scope.removeActivity(idx);

                swal.close();
                //}, 500);

            });
    }

    $scope.clearDiagram = function () {
        for (i = $scope.links.length - 1; i >= 0; i--) {
            $scope.links[i].line.remove();
            $scope.links.splice(i, 1);
        }
        for (i = $scope.nodes.length - 1; i >= 0; i--) {
            if ($scope.nodes[i].symbolCode == 'START' || $scope.nodes[i].symbolCode == 'END')
                continue;

            $("#" + $scope.nodes[i].element).remove();
            $scope.nodes.splice(i, 1);
        }
    }

    $scope.refreshDiagram = function () {
        // temp
        var xnodes = angular.copy($scope.nodes);
        var xlinks = angular.copy($scope.links);

        for (i = $scope.links.length - 1; i >= 0; i--) {
            $scope.links[i].line.remove();
            $scope.links.splice(i, 1);
        }
        for (i = $scope.nodes.length - 1; i >= 0; i--) {
            if ($scope.nodes[i].symbolCode == 'START' || $scope.nodes[i].symbolCode == 'END')
                continue;

            $("#" + $scope.nodes[i].element).remove();
            $scope.nodes.splice(i, 1);
        }
        // temp

        $scope.bindDiagram(xnodes, xlinks);

    }

    $scope.bindDiagram = function (nodes, links) {
        if (nodes == null) return;

        console.log(nodes);
        console.log(links);

        // node
        //$scope.nodes =angular.copy( nodes);
        $scope.inode = 3;
        for (i = 0; i < nodes.length; i++) {
            var node = nodes[i];
            var elm;
            if (node.symbolCode == 'START' || node.symbolCode == 'END') {
                elm = document.getElementById(node.element);
            } else if (node.symbolCode == 'ACTIVITY' && node.caption != 'Document Uploading') {
                elm = $scope.addNodeActivity(0, 0);
                var idx = $scope.getIndex(elm.id);

                var cap = $("#title-activity-" + idx);
                var info = $("#info-activity-" + idx);
                var id = document.getElementById("member-id-" + idx);
               
                cap.text(node.caption);
                info.attr("data-content", node.info);
                info.attr("data-original-title", node.caption);


            } else if (node.symbolCode == 'ACTIVITY' && node.caption == 'Document Uploading') {
                elm = document.getElementById('node-activity-2');

            }

            elm.style.left = node.posLeft == null ? 0 : node.posLeft;
            elm.style.top = node.posTop == null ? 0 : node.posTop;
            elm.style.width = node.width;
            elm.style.height = node.height;
        }

        // link
        //$scope.links = angular.copy(links);
        if (links != null) {
            for (i = 0; i < links.length; i++) {
                var link = links[i];
                var from = document.getElementById(link.elementFrom);
                var to = document.getElementById(link.elementTo);

                if (link.symbolCode == 'SUBMIT') {
                    $scope.addLinkSubmit(from, to);
                }
            }
        }
    }

    $scope.selectByMe = false;
    $scope.selectLink = function (elm) {
        if ($scope.links.length == 0)
            return;

        $scope.selectByMe = true;

        var idx = $scope.getIndex(elm);

        if ($scope.selectedLinkNoIdx != -1)
            $scope.links[$scope.selectedLinkNoIdx].line.setOptions({ size: 2, dash: false });

        var idxs = [];
        for (i = 0; i < $scope.links.length; i++) {
            if ($scope.links[i].elementFrom == elm || $scope.links[i].elementTo == elm)
                idxs.push(i);
        }

        if ($scope.selectedLinkNodeNo != idx) {
            $scope.selectedLinkNo = -1;
            $scope.selectedLinkNodeNo = idx;
        }

        $scope.selectedLinkNo++;
        if ($scope.selectedLinkNo >= idxs.length)
            $scope.selectedLinkNo = 0;

        i = idxs[$scope.selectedLinkNo];
        if (i != undefined) {
            $scope.links[i].line.setOptions({ size: 4, dash: { animation: true } });
            $scope.selectedLinkNoIdx = i;
        }

    }

    $scope.releaseAllSelected = function () {
        if ($scope.selectedLinkNoIdx != -1 && !$scope.selectByMe) {
            $scope.links[$scope.selectedLinkNoIdx].line.setOptions({ size: 2, dash: false });
            $scope.selectedLinkNoIdx = -1;
            $scope.selectedLinkNo = -1;
            $scope.selectedLinkNodeNo = -1;
        }
        $scope.selectByMe = false;
    }


    $("html").keyup(function (e) {
            console.log(e.key);
        if (e.key == 'x' || e.key == 'X' || e.key == 'Delete' || e.key == 'Backspace') {
            if ($scope.selectedLinkNoIdx != -1) {``
                swal({
                    title: "Confirmation",
                    text: "This link will be deleted, are you sure?",
                    type: "warning",
                    showCancelButton: true,
                    closeOnConfirm: false,
                    confirmButtonColor: "#2196F3",
                    showLoaderOnConfirm: true,
                    confirmButtonText: "Yes, delete it!",
                    cancelButtonText: "No, cancel please!",
                },
                    function (isConfirm) {
                        //setTimeout(function () {
                        if (isConfirm) {
                            var i = $scope.selectedLinkNoIdx;
                            $scope.releaseAllSelected();
                            $scope.links[i].line.remove();
                            $scope.links.splice(i, 1);
                            swal.close();
                        }
                        //}, 500);

                    });
            }
        } else if (e.key == 'c' || e.key == 'C') {
            if ($scope.selectedLinkNoIdx != -1) {
                var idx = $scope.selectedLinkNoIdx;
                if ($scope.links[idx].symbolCode == 'SUBMITCASE')
                    $scope.popupLinkCase(idx);
                else if ($scope.links[idx].symbolCode == 'ALTER')
                    $scope.popupLinkAlter(idx);
            }
        }
    });

    $scope.submitFlow = function () {
        for (i = 0; i < $scope.nodes.length; i++) {
            var node = $scope.nodes[i];
            var elm = document.getElementById(node.element);
            var fromIdx = $scope.getIndex(elm.id);

            node.Operator = "";
            if (node.symbolCode == 'START') {
                var cap = $("#title-start-" + fromIdx);

                node.caption = cap.text();
                var color = $scope.rgb2hex($("#start-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#start-" + fromIdx).css("color"));
                node.textColor = color;
            } else if (node.symbolCode == 'END') {
                var cap = $("#title-end-" + fromIdx);

                node.caption = cap.text();
                var color = $scope.rgb2hex($("#end-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#end-" + fromIdx).css("color"));
                node.textColor = color;
            } else if (node.symbolCode == 'ACTIVITY') {
                var cap = $("#title-activity-" + fromIdx);
                var info = $("#info-activity-" + fromIdx).attr("data-content");
                node.caption = cap.text();
                node.info = info;
                var color = $scope.rgb2hex($("#node-activity-caption-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-activity-caption-" + fromIdx).css("color"));
                node.textColor = color;
            }

            node.posLeft = elm.style.left;
            node.posTop = elm.style.top;
            node.width = elm.style.width;
            node.height = elm.style.height;
        }

        return { nodes: $scope.nodes, links: $scope.links };
    }

    $scope.rgb2hex = function (rgb) {
        if (rgb.search("rgb") == -1) {
            return rgb;
        }
        else if (rgb == 'rgba(0, 0, 0, 0)') {
            return 'transparent';
        }
        else {
            rgb = rgb.match(/^rgba?\((\d+),\s*(\d+),\s*(\d+)(?:,\s*(\d+))?\)$/);
            function hex(x) {
                return ("0" + parseInt(x).toString(16)).slice(-2);
            }
            return "#" + hex(rgb[1]) + hex(rgb[2]) + hex(rgb[3]);
        }
    }

    $scope.initDiagram = function () {
    }

    $scope.getIndex = function (elmName) {
        var dashIdx = elmName.lastIndexOf("-") + 1;
        return elmName.substring(dashIdx, dashIdx + 5);
    }
    $scope.getFileName = function (fileName) {
        var dashIdx = fileName.lastIndexOf("/") + 1;
        return fileName.substring(dashIdx, dashIdx + 20);
    }

});

$(function () {

    $.contextMenu({
        selector: '.context-menu-one',
        callback: function (key, options) {
            if (key == 'addactivity') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.addNodeActivity(0, 0);
                });
            }  else if (key == 'refresh') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.refreshDiagram();
                });
            }

        },
        items: {
            "addactivity": { name: "Add Activity" }
        }
    });

    $('.context-menu-one').on('click', function (e) {
        var scope = angular.element(document.getElementById("xdiagramController")).scope();
    })
});


function canvasScroll() {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.refreshLines();
    });
}


function removeActivityConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeActivityConfirmation(idx);
    });
}

function selectLink(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.selectLink(idx);
    });
}


