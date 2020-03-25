myApp.controller("xdiagramController", function ($scope, Upload, $location, $http, $filter) {

    // modal member
    $scope.member = {};
    $scope.members = [];
    $scope.memberCount = [];
    $scope.paging = [];
    $scope.kriteria = "";
    $scope.page = 1;
    $scope.row = 20;
    $scope.currPage = 0;
    $scope.index = 0;
    // modal member

    $scope.nodeItem = { element: '', memberId: 0, symbolCode: '', caption: '', info: '', Operator: '', value: 0, textColor: '', backColor: '', posLeft: 0, posTop: 0, width: 0, height: 0, member: { number: '', name: '', email: '', imageProfile: '' } };
    $scope.nodes = [];
    $scope.linkItem = { elementFrom: '', elementTo: '', firstNode: '', endNode:'', symbolCode: '', caption: '', Operator: '', value: 0, line: {} };
    $scope.links = [];
    $scope.inode = 0;
    $scope.elmActivityName = "node-activity";
    $scope.elmTitleName = "title-activity";
    $scope.elmDecisionName = "node-decision";
    $scope.elmTransferName = "node-transfer";
    $scope.elmCaseName = "node-case";
    $scope.elmPararrelName = "node-pararrel";

    $scope.linkTypeSubmit = "grid";
    $scope.linkTypeRevisi = "straight";
    $scope.linkTypeReject = "fluid";
    $scope.linkTypeAlter = "straight";
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

        var line = new LeaderLine(
            document.getElementById('start-0'),
            document.getElementById('submit-start'),
            {
                endPlug: 'behind',
                hide: true,
                path: $scope.linkTypeSubmit,
                dash: { animation: true },
                size: 2,
                color: $scope.linkColorSubmit,
            });

        $("#start-0").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });
        $("#submit-start").draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                line.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                line.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";

            }
        });

        $scope.addNode('start-0', 'START', 'Start');

        $("#end-1").droppable({
            drop: function (event, ui) {
                var fromIdx = $scope.getIndex(ui.draggable.context.id);
                var from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                var to = document.getElementById("end-1");
                $scope.endNode = to;
                console.log($scope.endNode);

                if (ui.draggable.context.id == 'submit-' + fromIdx && fromIdx!='start') {
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
    }

    $scope.initActivity = function (idNo) {
        var activityId = $scope.elmActivityName + "-" + idNo;
        var submitId = "submit-" + idNo;
        var rejectId = "reject-" + idNo;
        var revisiId = "revisi-" + idNo;
        var alterId = "alter-" + idNo;

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);
        var reject = document.getElementById(rejectId);
        var revisi = document.getElementById(revisiId);
        var alter = document.getElementById(alterId);

        var
        lineSubmit = new LeaderLine(activity, submit, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorSubmit,
        }),
        lineReject = new LeaderLine(activity, reject, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorReject,
        }),
        lineRevisi = new LeaderLine(activity, revisi, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorRevisi,
        }),
        lineAlter = new LeaderLine(activity, alter, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorAlter,
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
        $("#" + rejectId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineReject.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineReject.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });
        $("#" + revisiId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineRevisi.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineRevisi.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });
        $("#" + alterId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineAlter.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineAlter.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });

        $("#" + activityId).droppable({
            drop: function (event, ui) {
                var fromIdx = $scope.getIndex(ui.draggable.context.id);
                var from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                var to = document.getElementById($scope.elmActivityName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-start') {
                    from = document.getElementById("start-0");
                    $scope.firstNode = to;
                    $scope.addLinkSubmit(from, to);
                } else if (ui.draggable.context.id == 'submit-' + fromIdx) {
                    $scope.addLinkSubmit(from, to);
                } /*else if (ui.draggable.context.id == 'reject-' + fromIdx) {
                    $scope.addLinkReject(from, to);
                }*/ else if (ui.draggable.context.id == 'revisi-' + fromIdx) {
                    $scope.addLinkRevisi(from, to);
                } else if (ui.draggable.context.id == 'alter-' + fromIdx) {
                    $scope.addLinkAlter(from, to);
                } else if (ui.draggable.context.id == 'submit-case-' + fromIdx) {
                    from = document.getElementById($scope.elmCaseName + "-" + fromIdx);
                    $scope.addLinkSubmitCase(from, to);
                } else if (ui.draggable.context.id == 'yes-' + fromIdx) {
                    from = document.getElementById($scope.elmDecisionName + "-" + fromIdx);
                    $scope.addLinkYes(from, to);
                } else if (ui.draggable.context.id == 'no-' + fromIdx) {
                    from = document.getElementById($scope.elmDecisionName + "-" + fromIdx);
                    $scope.addLinkNo(from, to);
                } else if (ui.draggable.context.id == 'submit-pararrel-' + fromIdx) {
                    from = document.getElementById($scope.elmPararrelName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                } else if (ui.draggable.context.id == 'submit-transfer-' + fromIdx) {
                    from = document.getElementById($scope.elmTransferName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                }
            },

        });

        $("#" + activityId).draggable({
            //cancel: " .title-activity",
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $("#" + activityId).resizable({
            resize: function (event, ui) {
                $scope.refreshLines()
            },
        });
    }

    $scope.initDecision = function (idNo) {
        var activityId = $scope.elmDecisionName + "-" + idNo;
        var yesId = "yes-" + idNo;
        var noId = "no-" + idNo;

        var activity = document.getElementById(activityId);
        var yes = document.getElementById(yesId);
        var no = document.getElementById(noId);

        var
        lineYes = new LeaderLine(activity, yes, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorYes,
        }),
        lineNo = new LeaderLine(activity, no, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorNo,
        });

        $("#" + yesId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineYes.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineYes.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });
        $("#" + noId).draggable({
            create: function (event, ui) { },
            drag: function (event, ui) {
                lineNo.position().show();
            },
            start: function (event, ui) { },
            stop: function (event, ui) {
                lineNo.hide('none');
                event.target.style.left = "";
                event.target.style.top = "";
            }
        });


        $("#" + activityId).droppable({
            drop: function (event, ui) {
                var fromIdx = $scope.getIndex(ui.draggable.context.id);
                var from;
                var to = document.getElementById($scope.elmDecisionName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-' + fromIdx && fromIdx != 'start') {
                    from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                } else if (ui.draggable.context.id == 'yes-' + fromIdx) {
                    from = document.getElementById($scope.elmDecisionName + "-" + fromIdx);
                    $scope.addLinkYes(from, to);
                } else if (ui.draggable.context.id == 'no-' + fromIdx) {
                    from = document.getElementById($scope.elmDecisionName + "-" + fromIdx);
                    $scope.addLinkNo(from, to);
                }
            },

        });

        $("#" + activityId).draggable({
            //cancel: " .title-decision",
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $("#" + activityId).resizable({
            resize: function (event, ui) {
                $scope.refreshLines()
            },
        });
    }

    $scope.initTransfer = function (idNo) {
        var activityId = $scope.elmTransferName + "-" + idNo;
        var submitId = "submit-transfer-" + idNo;

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);

        var
        lineSubmit = new LeaderLine(activity, submit, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
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
                var from;
                var to = document.getElementById($scope.elmTransferName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-' + fromIdx && fromIdx != 'start') {
                    from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                }
            },

        });

        $("#" + activityId).draggable({
            //cancel: " .title-decision",
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $("#" + activityId).resizable({
            resize: function (event, ui) {
                $scope.refreshLines()
            },
        });
    }

    $scope.initCase = function (idNo) {
        var activityId = $scope.elmCaseName + "-" + idNo;
        var submitId = "submit-case-" + idNo;

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);

        var
        lineSubmit = new LeaderLine(activity, submit, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
            dash: { animation: true },
            size: 2,
            color: $scope.linkColorSubmitCase,
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
                var from;
                var to = document.getElementById($scope.elmCaseName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-' + fromIdx && fromIdx != 'start') {
                    from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                }
            },

        });

        $("#" + activityId).draggable({
            //cancel: " .title-decision",
            create: function (event, ui) { },
            drag: function (event, ui) {
                $scope.refreshLines();
            },
            start: function (event, ui) { },
            stop: function (event, ui) { }
        });

        $("#" + activityId).resizable({
            resize: function (event, ui) {
                $scope.refreshLines()
            },
        });
    }

    $scope.initPararrel = function (idNo) {
        var activityId = $scope.elmPararrelName + "-" + idNo;
        var submitId = "submit-pararrel-" + idNo;

        var activity = document.getElementById(activityId);
        var submit = document.getElementById(submitId);

        var
        lineSubmit = new LeaderLine(activity, submit, {
            endPlug: 'behind',
            hide: true,
            path: 'straight',
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
                var from;
                var to = document.getElementById($scope.elmPararrelName + "-" + idNo);
                if (ui.draggable.context.id == 'submit-' + fromIdx && fromIdx != 'start') {
                    from = document.getElementById($scope.elmActivityName + "-" + fromIdx);
                    $scope.addLinkSubmit(from, to);
                }
            },

        });

        $("#" + activityId).draggable({
            //cancel: " .title-decision",
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

        var html = document.getElementById($scope.elmActivityName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmActivityName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initActivity($scope.inode);

        $scope.addNode($scope.elmActivityName + '-' + $scope.inode, 'ACTIVITY', 'Activity');
        // Popover
        $('[data-popup="popover"]').popover();
        // Tooltip
        $('[data-popup="tooltip"]').tooltip();

        return newDiv;
    }

    $scope.addNodeDecision = function (left, top) {
        var canvas = document.getElementById('canvas');

        var html = document.getElementById($scope.elmDecisionName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmDecisionName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initDecision($scope.inode);

        $scope.addNode($scope.elmDecisionName + '-' + $scope.inode, 'DECISION', 'Decision');
        // Popover
        $('[data-popup="popover"]').popover();
        // Tooltip
        $('[data-popup="tooltip"]').tooltip();

        return newDiv;
    }

    $scope.addNodeTransfer = function (left, top) {
        var canvas = document.getElementById('canvas');

        var html = document.getElementById($scope.elmTransferName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmTransferName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initTransfer($scope.inode);

        $scope.addNode($scope.elmTransferName + '-' + $scope.inode, 'TRANSFER', 'Transfer');
        // Popover
        $('[data-popup="popover"]').popover();
        // Tooltip
        $('[data-popup="tooltip"]').tooltip();

        return newDiv;
    }

    $scope.addNodeCase = function (left, top) {
        var canvas = document.getElementById('canvas');

        var html = document.getElementById($scope.elmCaseName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmCaseName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initCase($scope.inode);

        $scope.addNode($scope.elmCaseName + '-' + $scope.inode, 'CASE', 'Case');
        // Popover
        $('[data-popup="popover"]').popover();
        // Tooltip
        $('[data-popup="tooltip"]').tooltip();

        return newDiv;
    }

    $scope.addNodePararrel = function (left, top) {
        var canvas = document.getElementById('canvas');

        var html = document.getElementById($scope.elmPararrelName + "-xx").outerHTML;
        html = html.replace(/-xx-/g, $scope.inode);
        html = html.replace(/-xx/g, "-" + $scope.inode);
        html = html.replace(/-yy/g, "");
        $("#canvas").append(html);

        var newDiv = document.getElementById($scope.elmPararrelName + "-" + $scope.inode);
        newDiv.style.display = "";
        newDiv.style.left = left;
        newDiv.style.top = top;

        $scope.initPararrel($scope.inode);

        $scope.addNode($scope.elmPararrelName + '-' + $scope.inode, 'PARALLEL', 'Parallel');
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

        if (elmFrom == "start-0") {
            $scope.firstNode =  elmTo;
        }
        link.firstNode = $scope.firstNode;
        link.endNode = "end-1";
        link.elementFrom = elmFrom;
        link.elementTo = elmTo;
        link.symbolCode = sym;
        link.caption = cap;
        link.line = line;

        console.log("add link ");
        console.log(link);
        if (!isready)
            $scope.links.push(link);
    }

    $scope.addLinkSubmit = function (elmFrom, elmTo) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
            {
                endPlug: $scope.linkEndPlug, path: $scope.linkTypeSubmit, size: 2, color: $scope.linkColorSubmit, fontSize: $scope.linkLabelFontSize
            });

        console.log("ADD LINK SUBMIT :: " + $scope.firstNode+" "+$scope.endNode);
        $scope.addLink(elmFrom.id, elmTo.id, $scope.firstNode.id, $scope.endNode.id, 'SUBMIT', '', line);
    }

    $scope.addLinkSubmit = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeSubmit, size: 2, color: $scope.linkColorSubmit, fontSize: $scope.linkLabelFontSize
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement,'SUBMIT', '', line);
    }
    $scope.addLinkReject = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeReject, size: 2, color: $scope.linkColorReject,
                                    middleLabel: LeaderLine.pathLabel({ text: 'Reject', color: $scope.linkColorReject, fontSize: $scope.linkLabelFontSize, })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'REJECT', 'Reject', line);
    }
    $scope.addLinkRevisi = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeRevisi, size: 2, color: $scope.linkColorRevisi,
                                    middleLabel: LeaderLine.pathLabel({ text: 'Revision', color: $scope.linkColorRevisi, fontSize: $scope.linkLabelFontSize })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'REVISI', 'Revision', line);
    }
    $scope.addLinkYes = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeYes, size: 2, color: $scope.linkColorYes,
                                    middleLabel: LeaderLine.pathLabel({ text: 'Yes', color: $scope.linkColorYes, fontSize: $scope.linkLabelFontSize })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'YES', "Yes", line);
    }
    $scope.addLinkNo = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeNo, size: 2, color: $scope.linkColorNo,
                                    middleLabel: LeaderLine.pathLabel({ text: 'No', color: $scope.linkColorNo, fontSize: $scope.linkLabelFontSize })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'NO', "No", line);
    }
    $scope.addLinkAlter = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeAlter, size: 2, color: $scope.linkColorAlter,
                                    middleLabel: LeaderLine.pathLabel({ text: 'Period?', color: $scope.linkColorAlter, fontSize: $scope.linkLabelFontSize })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'ALTER', "Period?", line);
    }
    $scope.addLinkSubmitCase = function (elmFrom, elmTo, frstNode, endElement) {
        if (elmFrom.id == elmTo.id) return;
        var line = new LeaderLine(elmFrom, elmTo,
                                {
                                    endPlug: $scope.linkEndPlug, path: $scope.linkTypeSubmitCase, size: 2, color: $scope.linkColorSubmitCase,
                                    middleLabel: LeaderLine.pathLabel({ text: 'Expression?', color: $scope.linkColorSubmitCase, fontSize: $scope.linkLabelFontSize })
                                });
        $scope.addLink(elmFrom.id, elmTo.id, frstNode, endElement, 'SUBMITCASE', 'Expression?', line);
    }

    $scope.removeActivity = function (idx) {
        $scope.removeNode($scope.elmActivityName + "-" + idx);
    }

    $scope.removeDecision = function (idx) {
        $scope.removeNode($scope.elmDecisionName + "-" + idx);
    }

    $scope.removeTransfer = function (idx) {
        $scope.removeNode($scope.elmTransferName + "-" + idx);
    }


    $scope.removeCase = function (idx) {
        $scope.removeNode($scope.elmCaseName + "-" + idx);
    }

    $scope.removePararrel = function (idx) {
        $scope.removeNode($scope.elmPararrelName + "-" + idx);
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

    $scope.removeDecisionConfirmation = function (idx) {
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
                $scope.removeDecision(idx);

            swal.close();
            //}, 500);

        });
    }

    $scope.removeTransferConfirmation = function (idx) {
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
                $scope.removeTransfer(idx);

            swal.close();
            //}, 500);

        });
    }

    $scope.removeCaseConfirmation = function (idx) {
        swal({
            title: "Confirmation",
            text: "This case node will be deleted, are you sure?",
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
                $scope.removeCase(idx);

            swal.close();
            //}, 500);

        });
    }

    $scope.removePararrelConfirmation = function (idx) {
        swal({
            title: "Confirmation",
            text: "This parallel node will be deleted, are you sure?",
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
                $scope.removePararrel(idx);

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

        // node
        //$scope.nodes =angular.copy( nodes);
        $scope.inode = 2;
        for (i = 0; i < nodes.length; i++) {
            var node = nodes[i];

            if (node.symbolCode == 'START' || node.symbolCode == 'END') {
                elm = document.getElementById(node.element);
            } else if (node.symbolCode == 'PARALLEL') {
                elm = $scope.addNodePararrel(0, 0);
            } else if (node.symbolCode == 'ACTIVITY') {
                elm = $scope.addNodeActivity(0, 0);
                var idx = $scope.getIndex(elm.id);

                var cap = $("#title-activity-" + idx);
                var info = $("#info-activity-" + idx);
                var id = document.getElementById("member-id-" + idx);
                id.value = node.memberId;
                cap.text(node.caption);
                info.attr("data-content", node.info);
                info.attr("data-original-title", node.caption);

                if (node.member != null) {
                    var foto = document.getElementById("photo-profile-" + idx);
                    foto.src = "/Images/Member/" + node.member.imageProfile;
                    $("#member-number-" + idx).text(node.member.number);
                    $("#member-name-" + idx).text(node.member.name);
                    $("#member-email-" + idx).text(node.member.email);
                }

            } else if (node.symbolCode == 'DECISION') {
                elm = $scope.addNodeDecision(0, 0);
                var idx = $scope.getIndex(elm.id);

                var cap = $("#title-decision-" + idx);
                var info = $("#info-decision-" + idx);
                cap.text(node.caption);
                info.attr("data-content", node.info);
                info.attr("data-original-title", node.caption);

                $("#decision-descr-" + idx).text('value ' + node.Operator + ' ' + node.value);

                var val = document.getElementById("decision-value-" + idx);
                val.value = node.value;
                var oprt = document.getElementById("decision-operator-" + idx);
                oprt.value = node.Operator;

            } else if (node.symbolCode == 'TRANSFER') {
                elm = $scope.addNodeTransfer(0, 0);
                var idx = $scope.getIndex(elm.id);

                var cap = $("#title-transfer-" + idx);
                var info = $("#info-transfer-" + idx);
                cap.text(node.caption);
                info.attr("data-content", node.info);
                info.attr("data-original-title", node.caption);

                $("#transfer-descr-" + idx).text('amount = ' + node.value);

                var val = document.getElementById("transfer-value-" + idx);
                val.value = node.value;

            } else if (node.symbolCode == 'CASE') {
                elm = $scope.addNodeCase(0, 0);
                var idx = $scope.getIndex(elm.id);

                var cap = $("#title-case-" + idx);
                var info = $("#info-case-" + idx);
                cap.text(node.caption);
                info.attr("data-content", node.info);
                info.attr("data-original-title", node.caption);

                $("#case-descr-" + idx).text(node.value);
            }

            elm.style.left = node.posLeft;
            elm.style.top = node.posTop;
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
                } if (link.symbolCode == 'REJECT') {
                    $scope.addLinkReject(from, to);
                } if (link.symbolCode == 'REVISI') {
                    $scope.addLinkRevisi(from, to);
                } if (link.symbolCode == 'YES') {
                    $scope.addLinkYes(from, to);
                } if (link.symbolCode == 'NO') {
                    $scope.addLinkNo(from, to);
                } if (link.symbolCode == 'ALTER') {
                    $scope.addLinkAlter(from, to);

                    var val = link.value + ' ' + link.Operator + (link.value > 1 ? 'S' : '');
                    if (link.Operator == null) {
                        val = "Period?";
                        link.Operator = "HOUR";
                    }
                    $scope.links[i].line.setOptions({ middleLabel: LeaderLine.pathLabel({ text: val, color: $scope.linkColorSubmitCase, fontSize: '8pt' }) });
                    $scope.links[i].value = link.value;
                    $scope.links[i].Operator = link.Operator;
                } if (link.symbolCode == 'SUBMITCASE') {
                    $scope.addLinkSubmitCase(from, to);

                    var val = link.Operator + ' ' + link.value;
                    if (link.Operator == 'ELSE')
                        val = link.Operator;
                    $scope.links[i].line.setOptions({ middleLabel: LeaderLine.pathLabel({ text: val, color: $scope.linkColorSubmitCase, fontSize: '8pt' }) });
                    $scope.links[i].value = link.value;
                    $scope.links[i].Operator = link.Operator;
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

    $("html").keypress(function (e) {
        if (e.key == 'x' || e.key == 'X') {
            if ($scope.selectedLinkNoIdx != -1) {
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
        //{ element: '', memberId: 0, symbolCode: '', caption: '', info:'', Operator:'', value:0, textColor: '', backColor: '', posleft: 0, postop: 0, width: 0, height: 0 };
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
                var id = document.getElementById("member-id-" + fromIdx);
                node.memberId = id.value;
                node.caption = cap.text();
                node.info = info;
                var color = $scope.rgb2hex($("#node-activity-caption-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-activity-caption-" + fromIdx).css("color"));
                node.textColor = color;

                // collect member
                var foto = document.getElementById("photo-profile-" + fromIdx);
                node.member.number = $("#member-number-" + fromIdx).text();
                node.member.name = $("#member-name-" + fromIdx).text();
                node.member.email = $("#member-email-" + fromIdx).text();
                node.member.imageProfile = $scope.getFileName(foto.src);
            } else if (node.symbolCode == 'DECISION') {
                var cap = $("#title-decision-" + fromIdx);
                var info = $("#info-decision-" + fromIdx).attr("data-content");
                var operator = document.getElementById("decision-operator-" + fromIdx);
                var value = document.getElementById("decision-value-" + fromIdx);
                node.caption = cap.text();
                node.info = info;
                node.value = value.value;
                node.Operator = operator.value;
                var color = $scope.rgb2hex($("#node-decision-caption-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-decision-caption-" + fromIdx).css("color"));
                node.textColor = color;
            } else if (node.symbolCode == 'TRANSFER') {
                var cap = $("#title-transfer-" + fromIdx);
                var info = $("#info-transfer-" + fromIdx).attr("data-content");
                //var operator = document.getElementById("transfer-operator-" + fromIdx);
                var value = document.getElementById("transfer-value-" + fromIdx);
                node.caption = cap.text();
                node.info = info;
                node.value = value.value;
                //node.Operator = operator.value;
                var color = $scope.rgb2hex($("#node-transfer-caption-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-transfer-caption-" + fromIdx).css("color"));
                node.textColor = color;
            } else if (node.symbolCode == 'CASE') {
                var cap = $("#title-case-" + fromIdx);
                var info = $("#info-case-" + fromIdx).attr("data-content");
                var expression = $("#case-descr-" + fromIdx);
                node.caption = cap.text();
                node.info = info;
                node.value = expression.text();
                node.Operator = "";
                var color = $scope.rgb2hex($("#node-case-caption-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-case-caption-" + fromIdx).css("color"));
                node.textColor = color;
            } else if (node.symbolCode == 'PARALLEL') {
                var cap = $("#title-pararrel-" + fromIdx);
                node.caption = cap.text();
                node.Operator = "";
                var color = $scope.rgb2hex($("#node-pararrel-" + fromIdx).css("background-color"));
                node.backColor = color;
                color = $scope.rgb2hex($("#node-pararrel-" + fromIdx).css("color"));
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

    /*--------------------------------------------------------------
        POPUP MEMBER
    --------------------------------------------------------------*/
    $scope.activityNo = 0;
    $scope.lookupMember = function (idx) {
        $scope.activityNo = idx;
        $("#modal_select_member").modal("show");
        $scope.members = [];
    }
    $scope.setMember = function (idxMember) {
        var idx = $scope.activityNo;
        var foto = document.getElementById("photo-profile-" + idx);
        var id = document.getElementById("member-id-" + idx);

        var member = $scope.members[idxMember];//{ Id: 1, Number: '001', Name: 'Budi Mulyana', Email: 'a@yahoo.com', ImageProfile: 'budi.png' };

        foto.src = "/Images/Member/" + member.ImageProfile;
        $("#member-number-" + idx).text(member.Number);
        $("#member-name-" + idx).text(member.Name);
        $("#member-email-" + idx).text(member.Email);
        id.value = member.Id;
    }
    /*--------------------------------------------------------------
        END POPUP MEMBER
    --------------------------------------------------------------*/

    $scope.popupActivity = function (elm) {
        $scope.defineActivityElement = elm;
        var idx = $scope.getIndex(elm);
        $scope.activity.Name = $("#" + elm).text();
        $scope.activity.Info = $("#info-activity-" + idx).attr("data-content");
        $("#modal_select_activity").modal("show");
    }
    $scope.setActivity = function () {
        var elm = $scope.defineActivityElement;
        var idx = $scope.getIndex(elm);
        $("#" + elm).text($scope.activity.Name);
        $("#info-activity-" + idx).attr("data-content", $scope.activity.Info);
        $("#info-activity-" + idx).attr("data-original-title", $scope.activity.Name);

    }

    $scope.popupDecision = function (elm) {
        $scope.defineDecisionElement = elm;
        if ($scope.decOperators.length==0)
            $scope.decOperators=$scope.operators.filter((subject) => subject.Descr != 'ELSE')
        var idx = $scope.getIndex(elm);
        $scope.decision.Name = $("#" + elm).text();
        $scope.decision.Info = $("#info-decision-" + idx).attr("data-content");

        var val = document.getElementById("decision-value-" + idx);
        $scope.decision.Value = val.value;
        var oprt = document.getElementById("decision-operator-" + idx);

        $scope.decision.Operator = oprt.value;
        $('#decision-operator').val(oprt.value);
        $("#modal_select_decision").modal("show");
    }
    $scope.setDecision = function () {
        var elm = $scope.defineDecisionElement;
        var idx = $scope.getIndex(elm);
        $("#" + elm).text($scope.decision.Name);
        $("#info-decision-" + idx).attr("data-content", $scope.decision.Info);
        $("#info-decision-" + idx).attr("data-original-title", $scope.decision.Name);
        $("#decision-descr-" + idx).text('value ' + $scope.decision.Operator + ' ' + $scope.decision.Value);

        var val = document.getElementById("decision-value-" + idx);
        val.value = $scope.decision.Value;
        var oprt = document.getElementById("decision-operator-" + idx);
        oprt.value = $scope.decision.Operator;
    }

    $scope.popupTransfer = function (elm) {
        $scope.defineTransferElement = elm;
        if ($scope.decOperators.length == 0)
            $scope.decOperators = $scope.operators.filter((subject) => subject.Descr != 'ELSE')
        var idx = $scope.getIndex(elm);
        $scope.transfer.Name = $("#" + elm).text();
        $scope.transfer.Info = $("#info-transfer-" + idx).attr("data-content");

        var val = document.getElementById("transfer-value-" + idx);
        $scope.transfer.Value = val.value;
        $("#modal_select_transfer").modal("show");
    }
    $scope.setTransfer = function () {
        var elm = $scope.defineTransferElement;
        var idx = $scope.getIndex(elm);
        $("#" + elm).text($scope.transfer.Name);
        $("#info-transfer-" + idx).attr("data-content", $scope.transfer.Info);
        $("#info-transfer-" + idx).attr("data-original-title", $scope.transfer.Name);
        $("#transfer-descr-" + idx).text('amount = ' + $scope.transfer.Value);

        var val = document.getElementById("transfer-value-" + idx);
        val.value = $scope.transfer.Value;
    }

    $scope.popupCase = function (elm) {
        $scope.defineCaseElement = elm;
        var idx = $scope.getIndex(elm);
        $scope.case.Name = $("#" + elm).text();
        $scope.case.Info = $("#info-case-" + idx).attr("data-content");
        $scope.case.Expression = $("#case-descr-" + idx).text();
        $("#modal_select_case").modal("show");
    }
    $scope.setCase = function () {
        var elm = $scope.defineCaseElement;
        var idx = $scope.getIndex(elm);
        $("#" + elm).text($scope.case.Name);
        $("#info-case-" + idx).attr("data-content", $scope.case.Info);
        $("#info-case-" + idx).attr("data-original-title", $scope.case.Name);
        $("#case-descr-" + idx).text($scope.case.Expression);
    }

    $scope.popupLinkCase = function (idx) {
        $scope.defineLinkCaseElement = idx;
        $scope.linkcase.Operator = $scope.links[idx].Operator;
        $scope.linkcase.Value = $scope.links[idx].value;
        if ($scope.linkcase.Operator == '')
            $scope.linkcase.Operator = "=";
        $('#linkcase-operator').val($scope.linkcase.Operator);
        $('#linkcase-value').val($scope.linkcase.Value);
        $("#modal_select_linkcase").modal("show");
    }
    $scope.setLinkCase = function () {
        var idx = $scope.defineLinkCaseElement;
        $scope.links[idx].Operator = $scope.linkcase.Operator;
        $scope.links[idx].value = $scope.linkcase.Value;
        var val = $scope.linkcase.Operator + ' ' + $scope.linkcase.Value;
        if ($scope.linkcase.Operator == 'ELSE')
            val = $scope.linkcase.Operator;
        $scope.links[idx].line.setOptions({ middleLabel: LeaderLine.pathLabel({ text: val, color: $scope.linkColorSubmitCase, fontSize: '8pt' }) });
    }

    $scope.popupLinkAlter = function (idx) {
        $scope.defineLinkAlterElement = idx;
        $scope.linkalter.PeriodUnit = $scope.links[idx].Operator;
        $scope.linkalter.Period = $scope.links[idx].value;
        if ($scope.linkalter.PeriodUnit == '')
            $scope.linkalter.PeriodUnit = "HOUR";
        $('#linkalter-period').val($scope.linkalter.Period);
        $('#linkalter-periodunit').val($scope.linkalter.PeriodUnit);
        $("#modal_select_linkalter").modal("show");
    }
    $scope.setLinkAlter = function () {
        var idx = $scope.defineLinkAlterElement;
        $scope.links[idx].Operator = $scope.linkalter.PeriodUnit;
        $scope.links[idx].value = $scope.linkalter.Period;
        var val = $scope.linkalter.Period + ' ' + $scope.linkalter.PeriodUnit + ($scope.linkalter.Period > 1 ? 'S' : '');
        $scope.links[idx].line.setOptions({ middleLabel: LeaderLine.pathLabel({ text: val, color: $scope.linkColorSubmitCase, fontSize: '8pt' }) });
    }

    $scope.getIndex = function (elmName) {
        var dashIdx = elmName.lastIndexOf("-") + 1;
        return elmName.substring(dashIdx, dashIdx + 5);
    }
    $scope.getFileName = function (fileName) {
        var dashIdx = fileName.lastIndexOf("/") + 1;
        return fileName.substring(dashIdx, dashIdx + 20);
    }

    /*--------------------------------------------------------------
                POPUP MEMBER
    --------------------------------------------------------------*/
    $scope.memberIdx = -1;
    $scope.editMember = function (idx) {
        $scope.memberIdx = idx;
        $scope.members = [];
        $("#modal_select_member").modal("show");
    }

    $scope.setMember = function (idx) {
        $scope.users[$scope.memberIdx].UserId = $scope.members[idx].Id;
        $scope.users[$scope.memberIdx].Name = $scope.members[idx].Name;
        $scope.users[$scope.memberIdx].Email = $scope.members[idx].Email;
        $scope.users[$scope.memberIdx].Picture = $scope.members[idx].ImageProfile;
    }

    $scope.findMembers = function (kriteria, page, row) {
        $scope.members = [];
        $http.post('/Member/FindMembers', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.members = response.data.Items;
                $scope.index = row * (page - 1);
                $scope.paging = [];
                var jumlahData = response.data.Count;
                var jumlahPage = Math.ceil(jumlahData / $scope.row);
                for (var i = 1; i <= jumlahPage; i++) {
                    $scope.paging.push({ value: i, text: i });
                }
                $scope.page = "1";
                $scope.isView = true;
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }

    $scope.changePageMember = function (kriteria, page, row) {
        $scope.products = [];
        $http.post('/Member/FindMembers', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.members = response.data;
                $scope.index = row * (page - 1);
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }
    $scope.removeMember = function (idx) {
        $scope.users.splice(idx, 1);
    }
    $scope.popupMember = function () {
        $scope.memberIdx = -1;
        $scope.members = [];
    }
    /*--------------------------------------------------------------
    END POPUP MEMBER
    --------------------------------------------------------------*/

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
            } else if (key == 'adddecision') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.addNodeDecision(0, 0);
                });
            } else if (key == 'addtransfer') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.addNodeTransfer(0, 0);
                });
            } else if (key == 'addcase') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.addNodeCase(0, 0);
                });
            } else if (key == 'addpararrel') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.addNodePararrel(0, 0);
                });
            } else if (key == 'refresh') {
                var scope = angular.element(document.getElementById("xdiagramController")).scope();
                scope.$apply(function () {
                    scope.refreshDiagram();
                });
            }

        },
        items: {
            "addactivity": { name: "Add Activity" },
            "sep1": "---------",
            "adddecision": { name: "Add Decision" },
            "addtransfer": { name: "Add Transfer" },
            "addcase": { name: "Add Case" },
            "addpararrel": { name: "Add Parallel" },
            //"sep2": "---------",
            //"refresh": { name: "Refresh" },
            //copy: { name: "Copy", icon: "copy" },
            //"paste": { name: "Paste", icon: "paste" },
            //"delete": { name: "Delete", icon: "delete" },
            //"sep1": "---------",
            //"quit": {
            //    name: "Quit", icon: function () {
            //        return 'context-menu-icon context-menu-icon-quit';
            //    }
            //}
        }
    });

    $('.context-menu-one').on('click', function (e) {
        var scope = angular.element(document.getElementById("xdiagramController")).scope();
        console.log("scope.firstNode");
        console.log(scope.firstNode);
        console.log("scope.endNode");
        console.log(scope.endNode);

    })
});


function canvasScroll() {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.refreshLines();
    });
}

function lookupReceipent(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.lookupMember(idx);
    });
}

function popupActivity(elm) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.popupActivity(elm);
    });
}
function popupDecision(elm) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.popupDecision(elm);
    });
}
function popupTransfer(elm) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.popupTransfer(elm);
    });
}

function popupCase(elm) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.popupCase(elm);
    });
}

function removeActivityConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeActivityConfirmation(idx);
    });
}
function removeDecisionConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeDecisionConfirmation(idx);
    });
}
function removeTransferConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeTransferConfirmation(idx);
    });
}

function removeCaseConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeCaseConfirmation(idx);
    });
}
function removePararrelConfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removePararrelConfirmation(idx);
    });
}
function removeAlterfirmation(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.removeAlterfirmation(idx);
    });
}

function selectLink(idx) {
    var scope = angular.element(document.getElementById("xdiagramController")).scope();
    scope.$apply(function () {
        scope.selectLink(idx);
    });
}


