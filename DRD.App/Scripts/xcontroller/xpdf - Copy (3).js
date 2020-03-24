myApp.controller("xpdfController", function ($scope, $location, $http, $filter) {
    var annotationType = {
        POINTER: 0,
        PEN: 1,
        HIGHLIGHTER: 2,
        TEXT: 3,
        SIGNATURE: 4,
        INITIAL: 5,
        STAMP: 6,
        PRIVATESTAMP: 7
    };


    var annoLayerClass = "xannoLayer";
    var editTextClass = "text-tool";
    var signatureClass = "signature";
    var initialClass = "initial";
    var privateStampClass = "privatestamp";
    var stampClass = "stamp";
    var penClass = "pen";
    var svgPadding = 4.;
    var toolType = 0;
    var clickedPage = 1;

    var strokeWidth = 5;
    var bufferSize = 4;

    var svgElement;
    var rect = {};
    var path = null;
    var strPath;
    var colorDefault = 'red';
    var colorPen = 'red';
    var colorHighlight = 'orange';
    var textColor = 'red';
    var opacityColor = 1;
    var lineStraight = false;
    var buffer = [];
    var svgNo = 0;
    //var childNo = 0;
    var selectedNodeId;
    var lastMouseX = 0;
    var lastMouseY = 0;
    var selectedMouseDown;
    var isresize = false;
    var isselector = false;
    var penWidth = 4;
    var highlighterWidth = 12;
    var dropedToCenter = true;
    var insertImgNo = 0;
    $scope.transform = { textAbsRotation: "0", scaleX: 1, scaleY: 1, transX: 0, transY: 0 };

    var annoItem = {Id: 0, SvgId: '', Page: 0, AnnotateType: '', LeftPosition: 0, TopPosition: 0, WidthPosition: null, HeightPosition: null, Color: null, BackColor: null, Data: null, Data2: null, Rotation: 0, ScaleX: 1, ScaleY: 1, TransitionX: 0, TransitionY: 0, StrokeWidth: 4, Opacity: 1, CreatorId: null, ElementId: null, IsDeleted: false, Flag: 0, FlagCode: null, FlagDate: null, FlagImage: null, Annotate: {Number: null, Name: null, Foto: null}};//, Signature: null, Initial: null } };
    $scope.annoItems = [];
    var tmpPenAnnoItem = {};

    angular.element(document).ready(function () {

        //$(document).on('mousedown touchstart', ".xannoLayer", xannoLayerMouseDown);
        //$(document).on('mousedown touchstart', ".xannoLayer", xannoLayerMouseDown);
        document.addEventListener("mousedown", xannoLayerMouseDown);
        document.addEventListener("touchstart", xannoLayerMouseDown);
    });

    $scope.addAnnoItem = function (item) {
        item.SvgId = "svg" + svgNo;
        $scope.annoItems.push(item);
        //svgNo++;
    }
    $scope.removeAnnoItem = function (itemId) {
        var svgId = $("#" + itemId).parent()[0].id;
        $("#" + svgId).remove();
        for (i = $scope.annoItems.length - 1; i >= 0; i--) {
            if ($scope.annoItems[i].SvgId == svgId) {
                //$scope.annoItems.splice(i, 1);
                $scope.annoItems[i].IsDeleted = true;
                break;
            }
        }
    }
    $scope.cleanAnnoItem = function () {
        for (i = $scope.annoItems.length - 1; i >= 0; i--) {
            if ($scope.annoItems[i].IsDeleted) {
                $scope.annoItems.splice(i, 1);
            }
        }
    }
    $scope.findAnnoItem = function (svgId) {
        for (i = 0; i < $scope.annoItems.length ; i++) {
            if ($scope.annoItems[i].SvgId == svgId) {
                return i;
            }
        }
        return -1;
    }

    $scope.addAnnoSignature = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];
        //var trf = $scope.transform;
        //var trftext = 'transform: rotate(' + trf.textAbsRotation + 'deg) ' + 'scale(' + trf.scaleX + ', ' + trf.scaleY + ') ' + 'translate(' + trf.transX + ', ' + trf.transY + ')';
        var annolayer = createAnnoLayer(item.Page);
        var svg = '<div id="' + item.SvgId + '" class="svg" style="position:absolute; top:0px; left:0px; width:100%; height:100%;"></div>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);

        enableAnnoLayer(false);
        var cno = item.SvgId.replace('svg', '');
        var html;
        if ((item.Flag & 1) == 1) {
            html = document.getElementById(signatureClass + "-xxx").outerHTML;
            html = html.replace(/-xxx/g, "-" + cno);
        } else {
            html = document.getElementById(signatureClass + "-xx").outerHTML;
            html = html.replace(/-xx-/g, cno);
            html = html.replace(/-xx/g, "-" + cno);
        }
        var field = $('#' + svgElement.id).append(html).find('.' + signatureClass);
        if (item.WidthPosition != null)
            field.css({ 'width': item.WidthPosition + 'px' });
        if (item.HeightPosition != null)
            field.css({ 'height': item.HeightPosition + 'px' });

        field.show();

        signatureArrange(signatureClass, item.LeftPosition, item.TopPosition);
    }
    $scope.addAnnoInitial = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];
        var annolayer = createAnnoLayer(item.Page);
        var svg = '<div id="' + item.SvgId + '" style="position:absolute; top:0px; left:0px; width:100%; height:100%;"></div>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);

        enableAnnoLayer(false);
        var cno = item.SvgId.replace('svg', '');
        var html;
        if ((item.Flag & 1) == 1) {
            html = document.getElementById(initialClass + "-xxx").outerHTML;
            html = html.replace(/-xxx/g, "-" + cno);
        } else {
            html = document.getElementById(initialClass + "-xx").outerHTML;
            html = html.replace(/-xx-/g, cno);
            html = html.replace(/-xx/g, "-" + cno);
        }
        var field = $('#' + svgElement.id).append(html).find('.' + initialClass);
        if (item.WidthPosition != null)
            field.css({ 'width': item.WidthPosition + 'px' });
        if (item.HeightPosition != null)
            field.css({ 'height': item.HeightPosition + 'px' });
        field.show();

        signatureArrange(initialClass, item.LeftPosition, item.TopPosition);
    }
    $scope.addAnnoPrivateStamp = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];

        var annolayer = createAnnoLayer(item.Page);
        var svg = '<div id="' + item.SvgId + '" style="position:absolute; top:0px; left:0px; width:100%; height:100%;"></div>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);

        enableAnnoLayer(false);
        var cno = item.SvgId.replace('svg', '');
        var html;
        if ((item.Flag & 1) == 1) {
            html = document.getElementById(privateStampClass + "-xxx").outerHTML;
            html = html.replace(/-xxx/g, "-" + cno);
        } else {
            html = document.getElementById(privateStampClass + "-xx").outerHTML;
            html = html.replace(/-xx-/g, cno);
            html = html.replace(/-xx/g, "-" + cno);
        }
        var field = $('#' + svgElement.id).append(html).find('.' + privateStampClass);
        if (item.WidthPosition != null)
            field.css({ 'width': item.WidthPosition + 'px' });
        if (item.HeightPosition != null)
            field.css({ 'height': item.HeightPosition + 'px' });
        field.show();

        signatureArrange(privateStampClass, item.LeftPosition, item.TopPosition);
    }
    $scope.addAnnoStamp = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];

        var annolayer = createAnnoLayer(item.Page);
        var svg = '<div id="' + item.SvgId + '" style="position:absolute; top:0px; left:0px; width:100%; height:100%;"></div>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);
        enableAnnoLayer(false);
        var cno = item.SvgId.replace('svg', '');
        var text = '<img class="' + stampClass + '" id="' + stampClass + cno + '" src="/images/stamp/empty.png" style="left:0px; top:0px; height:100px; width:200px; padding: ' + svgPadding + 'px;" onclick="annoSelector(' + "'" + stampClass + cno + "'" + ')"/>';
        var field = $('#' + svgElement.id).append(text).find('.' + stampClass);

        if (item.WidthPosition != null)
            field.css({ 'width': item.WidthPosition + 'px' });
        if (item.HeightPosition != null)
            field.css({ 'height': item.HeightPosition + 'px' });
        field.show();

        signatureArrange(stampClass, item.LeftPosition, item.TopPosition);
    }
    $scope.addAnnoText = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];

        var annolayer = createAnnoLayer(item.Page);
        var svg = '<div id="' + item.SvgId + '" style="position:absolute; top:0px; left:0px; width:100%; height:100%;"></div>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);

        enableAnnoLayer(false);
        var cno = item.SvgId.replace('svg', '');
        var html = '<div class="' + editTextClass + '" id="' + editTextClass + cno + '" contenteditable="true" spellcheck="false" style="left:0px; top:0px; padding: ' + svgPadding + 'px;color:' + textColor + ';pointer-events:stroke;" onclick="annoSelector(' + "'" + editTextClass + cno + "'" + ')">' + (item.Data == null ? '' : item.Data) + '</div>';
        var field = $('#' + svgElement.id).append(html);

        document.getElementById(editTextClass + cno).addEventListener("input", onEditText, false);
        field.css({ 'left': item.LeftPosition + 'px' });
        field.css({ 'top': item.TopPosition + 'px' });
        if (item.WidthPosition != null)
            field.css({ 'width': item.WidthPosition + 'px' });
        if (item.HeightPosition != null)
            field.css({ 'height': item.HeightPosition + 'px' });
        if (!isAnnoElementEnable)
            $('.' + editTextClass).attr('contenteditable', 'false');
        var defFlag = dropedToCenter;
        dropedToCenter = false;
        signatureArrange(editTextClass, item.LeftPosition, item.TopPosition);
        dropedToCenter = defFlag;
    }
    $scope.addAnnoPen = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];

        var annolayer = createAnnoLayer(item.Page);
        var svg = '<svg id="' + item.SvgId + '" class="svg" style="position:absolute; top:0px; left:0px;width:100%;height:100%"></svg>';
        annolayer.append(svg);
        svgElement = document.getElementById(item.SvgId);
        var cno = item.SvgId.replace('svg', '');
        path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute("fill", "none");
        path.setAttribute("stroke", item.Color);
        path.setAttribute("stroke-width", item.StrokeWidth);
        path.setAttribute("style", "cursor:pointer;pointer-events:stroke;opacity:" + item.Opacity);
        //
        path.setAttribute('d', item.Data);
        path.setAttribute('id', penClass + cno);
        path.setAttribute('class', penClass);
        path.setAttribute('onclick', "annoSelector(" + "'" + penClass + cno + "'" + ")");
        $('#' + svgElement.id).append(path);
        var t = item.TopPosition - svgPadding;
        var l = item.LeftPosition - svgPadding;
        var w = item.WidthPosition + (svgPadding * 2);
        var h = item.HeightPosition + (svgPadding * 2);
        $('#' + svgElement.id).css({ 'top': t + 'px' });
        $('#' + svgElement.id).css({ 'left': l + 'px' });
        $('#' + svgElement.id).css({ 'width': w + 'px' });
        $('#' + svgElement.id).css({ 'height': h + 'px' });
        $('#' + svgElement.id).css({ 'pointer-events': 'none' });
        $('#' + svgElement.id)[0].setAttribute('preserveAspectRatio', "none");
        $('#' + svgElement.id)[0].setAttribute('viewBox', item.Data2);
    }
    $scope.insertPngToSvg = function () {
        var i = insertImgNo;
        if (i >= $scope.annoItems.length) {
            if ($scope.command=="DOWNLOAD")
                $scope.doDownload();
            else if ($scope.command == "PRINT")
                $scope.doPrint();

            return;
        }
        
        var anno = $scope.annoItems[i];
        if (anno.IsDeleted || !(anno.AnnotateType == annotationType.PEN || anno.AnnotateType == annotationType.HIGHLIGHTER)) {
            insertImgNo++;
            $scope.insertPngToSvg();
            return;
        }

        var svg = $("#" + anno.SvgId);
        var annolyr = $('#' + svg[0].parentElement.id);
        var l = svg.css('left');
        var t = svg.css('top');
        var w = svg.css('width');
        var h = svg.css('height');

        var svgString = new XMLSerializer().serializeToString(document.querySelector('#' + anno.SvgId));
        svgString = svgString.replace('left: ' + l, 'left: 0px').replace('top: ' + t, 'top: 0px');
        var canvas = document.getElementById('xcanvas');
        canvas.width = canvas.width;
        var ctx = canvas.getContext("2d");
        var svg = new Blob([svgString], { type: "image/svg+xml;charset=utf-8" }),
            domURL = self.URL || self.webkitURL || self,
            url = domURL.createObjectURL(svg),
            img = new Image;
        img.src = 'data:image/svg+xml,' + encodeURIComponent(svgString);//url;

        img.onload = function () {
            ctx.drawImage(img, 0, 0);
            var png = canvas.toDataURL("image/png");
            annolyr.append('<img id="xtmpimg' + i + '" class="xtmpimg" style="top:' + t + ';left:' + l + ';height:' + h + ';width:' + w + ';position:absolute;" src="' + png + '"/>');
            insertImgNo++;
            $scope.insertPngToSvg();
        };

    }
    $scope.removePngInSvg = function () {

    }

    var bindAnnoDataMember = function(i)
    {
        var item = $scope.annoItems[i];
        if (item.ElementId == null || !(item.AnnotateType == annotationType.SIGNATURE || item.AnnotateType == annotationType.INITIAL || item.AnnotateType == annotationType.PRIVATESTAMP))
            return;

        var no = item.SvgId.replace('svg', '');
        if ((item.Flag & 1) == 1) {
            var foto = document.getElementById("signed-" + no);
            foto.src = "/Images/Member/" + item.FlagImage;
            if (item.AnnotateType == annotationType.SIGNATURE || item.AnnotateType == annotationType.INITIAL)
                $("#signed-name-" + no).text(item.Annotate.Name);
            $("#signed-code-" + no).text(item.FlagCode);
        } else {
            var foto = document.getElementById("member-foto-" + no);
            foto.src = "/Images/Member/" + item.Annotate.Foto;
            $("#member-name-" + no).text(item.Annotate.Name+' | '+item.Annotate.Number);
        }
    }
    var bindAnnoDataStamp = function (i) {
        var item = $scope.annoItems[i];
        if (item.ElementId == null || item.AnnotateType != annotationType.STAMP)
            return;

        var no = item.SvgId.replace('svg', '');
        var foto = document.getElementById("stamp" + no);
        foto.src = "/Images/stamp/" + item.Annotate.Foto;
    }
    $scope.bindAnnotation = function (page) {
        dropedToCenter = false;
        for (i = 0; i < $scope.annoItems.length; i++) {
            var item = $scope.annoItems[i];
            if (item.Page == page && !item.IsDeleted) {
                if (item.AnnotateType == annotationType.PEN) {
                    $scope.addAnnoPen(i);
                } else if (item.AnnotateType == annotationType.SIGNATURE) {
                    $scope.addAnnoSignature(i);
                    bindAnnoDataMember(i);
                } else if (item.AnnotateType == annotationType.INITIAL) {
                    $scope.addAnnoInitial(i);
                    bindAnnoDataMember(i);
                } else if (item.AnnotateType == annotationType.PRIVATESTAMP) {
                    $scope.addAnnoPrivateStamp(i);
                    bindAnnoDataMember(i);
                } else if (item.AnnotateType == annotationType.STAMP) {
                    $scope.addAnnoStamp(i);
                    bindAnnoDataStamp(i);
                } else if (item.AnnotateType == annotationType.TEXT) {
                    $scope.addAnnoText(i);
                }
            }
        }
        dropedToCenter = true;
    }

    $scope.clickReload = function () {
        $scope.bindAnnotation();
    }

    var xannoLayerMouseDown = function (e) {
        
        var page = parseInt(e.target.getAttribute('page-no'));
        if (!$.isNumeric(page)) {
            $scope.clickObjectButtonPointer();
            enableAnnoLayer(false);
            return;
        }
        if (toolType == annotationType.PEN)
            penMouseDown(e, page);
        else if (toolType == annotationType.HIGHLIGHTER)
            penMouseDown(e, page);
        else if (toolType == annotationType.TEXT)
            textMouseClick(e, page);
        else if (toolType == annotationType.SIGNATURE)
            signatureMouseClick(e, page);
        else if (toolType == annotationType.INITIAL)
            initialMouseClick(e, page);
        else if (toolType == annotationType.PRIVATESTAMP)
            privateStampMouseClick(e, page);
        else if (toolType == annotationType.STAMP)
            stampMouseClick(e, page);
    }

    $scope.clickObjectButtonPointer = function () {
        $(".btn-anno").removeClass("btn-active");
        $("#pointer").addClass("btn-active");
        toolType = annotationType.POINTER;
    }
    $scope.clickObjectButton = function (obj) {
        $(".btn-anno").removeClass("btn-active");

        var type = annotationType.POINTER;

        if (obj.currentTarget.id == "pen" || obj.currentTarget.id == "pen2")
            type = annotationType.PEN;
        else if (obj.currentTarget.id == "highlighter" || obj.currentTarget.id == "highlighter2")
            type = annotationType.HIGHLIGHTER;
        else if (obj.currentTarget.id == "text")
            type = annotationType.TEXT;
        else if (obj.currentTarget.id == "signature")
            type = annotationType.SIGNATURE;
        else if (obj.currentTarget.id == "initial")
            type = annotationType.INITIAL;
        else if (obj.currentTarget.id == "pstamp")
            type = annotationType.PRIVATESTAMP;
        else if (obj.currentTarget.id == "stamp")
            type = annotationType.STAMP;

        $("#" + obj.currentTarget.id).addClass("btn-active");
        toolType = type;
    }
    $scope.clickPointer = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(false);
    }
    $scope.clickPen = function (isStraight, obj) {
        $scope.clickObjectButton(obj);
        strokeWidth = penWidth;
        colorDefault = colorPen;
        opacityColor = 1;
        lineStraight = isStraight;
        enableAnnoLayer(true);
    }
    $scope.clickHighlight = function (isStraight, obj) {
        $scope.clickObjectButton(obj);
        strokeWidth = highlighterWidth;
        colorDefault = colorHighlight;
        opacityColor = 0.4;
        lineStraight = isStraight;
        enableAnnoLayer(true);
    }
    $scope.clickText = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(true);
    }
    $scope.clickSignature = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(true);
    }
    $scope.clickInitial = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(true);
    }
    $scope.clickPrivateStamp = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(true);
    }
    $scope.clickStamp = function (obj) {
        $scope.clickObjectButton(obj);
        enableAnnoLayer(true);
    }

    var base64Pdf;

    $scope.getBase64Pdf = function () {
        return base64Pdf;
    }

    var downloadFileName = null;
    $scope.setDownloadFileName = function (fname) {
        downloadFileName = fname;
    }
    $scope.clickDownload = function () {
        $http.post('/document/SaveCxDownload', { docName: $scope.defaultDocumentName }).then(function (response) {
            if (response.data) {
                var val = response.data;
                if (val == -1) {
                    showInfo("Sorry you can't print this document, the print count is up");
                    return;
                } else if (val == -2) {
                    showInfo("Sorry you can't print this document, the document has expired");
                    return;
                } else if (val == -4) {
                    showInfo("Sorry you can't print this document, there is an error");
                    return;
                }
                insertImgNo = 0;
                $scope.command = "DOWNLOAD";
                $scope.insertPngToSvg();
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });        
    }
    $scope.clickPrint = function () {
        $http.post('/document/SaveCxPrint', { docName: $scope.defaultDocumentName }).then(function (response) {
            if (response.data) {
                var val = response.data;
                if (val == -1) {
                    showInfo("Sorry you can't print this document, the print count is up");
                    return;
                } else if (val == -2) {
                    showInfo("Sorry you can't print this document, the document has expired");
                    return;
                } else if (val == -4) {
                    showInfo("Sorry you can't print this document, there is an error");
                    return;
                }
                insertImgNo = 0;
                $scope.command = "PRINT";
                $scope.insertPngToSvg();
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }
    $scope.doDownload = function () {
        
        /*
        <select id="mode">
          <option value="avoid-all">Avoid-all</option>
          <option value="css">CSS</option>
          <option value="legacy">Legacy</option>
          <option value="specify">Specified elements (using before/after/avoid)</option>
        </select>
        */
        // Get the element.
        var element = document.getElementById('viewer');

        // Choose pagebreak options based on mode.
        var mode = 'specify'; //document.getElementById('xannoLayer1').value;
        var pagebreak = (mode === 'specify') ?
            { mode: '', before: '.before', after: '.after', avoid: '.avoid' } :
            { mode: mode };

        var annolay = document.getElementById("xannoLayer1");
        var w = pos2Float(annolay.style.width);
        var h = pos2Float(annolay.style.height);

        // open property page and close for set value.
        var prop = PDFViewerApplication.pdfDocumentProperties.open();
        PDFViewerApplication.pdfDocumentProperties.close();
        setTimeout(function () {
            var pageSize = $("#pageSizeField").text();
            var pos1 = pageSize.indexOf("(");
            var pos2 = pageSize.indexOf(")");
            var paper = pageSize.substr(pos1 + 1, pos2 - pos1 - 1);
            var papers = paper.split(',');
            if (papers.length == 1) {
                papers[1] = papers[0];
                papers[0] = 'a4';
            }
            html2pdf().from(element).set({
                filename: downloadFileName,
                pagebreak: pagebreak,
                jsPDF: {
                    orientation: papers[1].trim(),
                    unit: 'in',
                    format: papers[0].trim(),
                    compressPDF: false
                }
            }).save().then(function (e) { $(".xtmpimg").remove(); });
        },1000);
    }
    $scope.doPrint = function () {

        $("#viewer").printThis({
            //debug: false,               // show the iframe for debugging
            //importCSS: true,            // import page CSS
            //importStyle: false,         // import style tags
            //printContainer: true,       // grab outer container as well as the contents of the selector
            //loadCSS: "/Scripts/pdf.js/web/viewer.css",  // path to additional css file - use an array [] for multiple
            ////pageTitle: "",              // add title to print page
            //removeInline: false,        // remove all inline styles from print elements
            //printDelay: 333,            // variable print delay
            ////header: null,               // prefix to html
            ////footer: null,               // postfix to html
            //base: false,               // preserve the BASE tag, or accept a string for the URL
            //formValues: true,           // preserve input/form values
            canvas: true,              // copy canvas elements (experimental)
            //doctypeString: "...",       // enter a different doctype for older markup
            //removeScripts: false,       // remove script tags from print content
            //copyTagClasses: false       // copy classes from the html & body tag
        });
        return;

        /*
        <select id="mode">
          <option value="avoid-all">Avoid-all</option>
          <option value="css">CSS</option>
          <option value="legacy">Legacy</option>
          <option value="specify">Specified elements (using before/after/avoid)</option>
        </select>
        */
        // Get the element.
        var element = document.getElementById('viewer');

        // Choose pagebreak options based on mode.
        var mode = 'specify'; //document.getElementById('xannoLayer1').value;
        var pagebreak = (mode === 'specify') ?
            { mode: '', before: '.before', after: '.after', avoid: '.avoid' } :
            { mode: mode };

        var annolay = document.getElementById("xannoLayer1");
        var w = pos2Float(annolay.style.width);
        var h = pos2Float(annolay.style.height);

        // open property page and close for set value.
        var prop = PDFViewerApplication.pdfDocumentProperties.open();
        PDFViewerApplication.pdfDocumentProperties.close();
        setTimeout(function () {
            var pageSize = $("#pageSizeField").text();
            var pos1 = pageSize.indexOf("(");
            var pos2 = pageSize.indexOf(")");
            var paper = pageSize.substr(pos1 + 1, pos2 - pos1 - 1);
            var papers = paper.split(',');
            if (papers.length == 1) {
                papers[1] = papers[0];
                papers[0] = 'a4';
            }
            html2pdf().from(element).set({
                filename: downloadFileName,
                pagebreak: pagebreak,
                jsPDF: {
                    orientation: papers[1].trim(),
                    unit: 'in',
                    format: papers[0].trim(),
                    compressPDF: false
                }
            }).save().then(function (e) { $(".xtmpimg").remove(); });
        }, 1000);
    }


    $scope.annoSelector = function (id) {
        if (!isAnnoElementEnable) return;
        selectedNodeId = id;

        if ($.find('.svg-selection').length == 0) {
            var html = document.getElementById("svg-selection-xx").outerHTML;
            html = html.replace(/-xx/g, "");
            $('#panel-body').append(html);
        }

        var parent = $("#" + selectedNodeId).parent()[0].id
        svgElement = document.getElementById(parent);

        //WrapWithMoveAndResizeTool("#" + parent);
        $scope.annoSelectorCreateEvent();
        $scope.repositionSelector(id);

        if (id.startsWith(editTextClass)) {
            $('#' + id).blur();
        }

        dragElement(document.getElementById("rect-selection"));

    }
    $scope.repositionSelectorPath = function () {
        var tagrect = $('.svg-selection > rect');

        var rectattr = tagrect[0].getBoundingClientRect();
        w = parseFloat(rectattr.width) + svgPadding;
        h = parseFloat(rectattr.height) + svgPadding

        $('.resize-cursor-topcenter').attr('cx', (w / 2) + (svgPadding / 2));
        $('.resize-cursor-topright').attr('cx', w);
        $('.resize-cursor-right').attr('cx', w);
        $('.resize-cursor-right').attr('cy', (h / 2) + (svgPadding / 2));
        $('.resize-cursor-left').attr('cy', (h / 2) + (svgPadding / 2));

        $('.resize-cursor-bottomright').attr('cx', w);
        $('.resize-cursor-bottomright').attr('cy', h);

        $('.resize-cursor-bottomcenter').attr('cx', (w / 2) + (svgPadding / 2));
        $('.resize-cursor-bottomcenter').attr('cy', h);

        $('.resize-cursor-bottomleft').attr('cy', h);
    }
    $scope.repositionSelector = function (id) {
        var node = $('#' + id)[0];
        var parent = node.parentNode.parentNode;
        var sel = $('.svg-selection').prependTo('#' + parent.id);
        var selector = $('.svg-selection').show();

        var svg = document.getElementById(id);
        svg = svg.parentNode;
        var rect = svg.getBoundingClientRect();

        var w = rect.width + (svgPadding * 2);
        var h = rect.height + (svgPadding * 2);

        selector.css({ "width": w + 'px' })
        selector.css({ "height": h + 'px' });

        var top = svg.offsetTop;
        var left = svg.offsetLeft;
        if (left == undefined)
            left = pos2Float($('#' + svg.id).css('left'));
        if (top == undefined)
            top = pos2Float($('#' + svg.id).css('top'));

        selector.css({ 'top': top - svgPadding + 'px' });
        selector.css({ 'left': left - svgPadding + 'px' });

        var tagrect = $('.svg-selection > rect');
        tagrect.css({ 'width': w - (svgPadding * 2) + "px" });
        tagrect.css({ 'height': h - (svgPadding * 2) + "px" });

        $scope.repositionSelectorPath();

    }
    $scope.annoSelectorCreateEvent = function () {
        var classname = document.getElementsByClassName("circle-selection");
        for (var i = 0; i < classname.length; i++) {
            classname[i].addEventListener('mousedown', selectorMouseDown);
            classname[i].addEventListener('touchstart', selectorMouseDown);
        }
        // mouse
        //$(".circle-selection").addEventListener("mousedown", selectorMouseDown);
        document.addEventListener("mousemove", selectorMouseMove);
        document.addEventListener("mouseup", selectorMouseUp);
        document.addEventListener("mousedown", documentMouseDown);
        //$('.rect-selection').bind("dblclick", selectionDblClick);

        // pen
        //$(".circle-selection").addEventListener("touchstart", selectorMouseDown);
        document.addEventListener("touchmove", selectorMouseMove);
        document.addEventListener("touchend", selectorMouseUp);
        document.addEventListener("touchstart", documentMouseDown);

        var classname2 = document.getElementsByClassName("rect-selection");
        for (var i = 0; i < classname2.length; i++) {
            classname2[i].addEventListener('dblclick', selectionDblClick);
        }
        //$('.rect-selection').addEventListener("dblclick", selectionDblClick);
        // keyboard
        document.addEventListener("keydown", delKeyDown);
        
    }
    $scope.annoSelectorRemoveEvent = function () {
        var classname = document.getElementsByClassName("circle-selection");
        for (var i = 0; i < classname.length; i++) {
            classname[i].removeEventListener('mousedown', selectorMouseDown);
            classname[i].removeEventListener('touchstart', selectorMouseDown);
        }
        // mouse
        //$(".circle-selection").removeEventListener("mousedown", selectorMouseDown);
        document.removeEventListener("mousemove", selectorMouseMove);
        document.removeEventListener("mouseup", selectorMouseUp);
        document.removeEventListener("mousedown", documentMouseDown);

        // pen
        //$(".circle-selection").removeEventListener("touchstart", selectorMouseDown);
        document.removeEventListener("touchmove", selectorMouseMove);
        document.removeEventListener("touchend", selectorMouseUp);
        document.removeEventListener("touchstart", documentMouseDown);

        var classname2 = document.getElementsByClassName("rect-selection");
        for (var i = 0; i < classname2.length; i++) {
            classname2[i].removeEventListener('dblclick', selectionDblClick);
        }
        //$('.rect-selection').removeEventListener("dblclick", selectionDblClick);
        // keyboard
        document.removeEventListener("keydown", delKeyDown);
    }

    var pos2Float = function (pos) {
        return parseFloat(pos.replace('px', ''))
    }
    var resizeSelector = function (id, w, h, x, y) {
        var selected = $('#' + id);
        var svgselection = $('#svg-selection');
        var rectselection = $('#rect-selection');

        var minW = selected.css('min-width');
        var minH = selected.css('min-height');
        if (minW != undefined && pos2Float(minW) > 0)
            minW = pos2Float(minW);
        else
            minW = 10;
        if (minH != undefined && pos2Float(minH) > 0)
            minH = pos2Float(minH);
        else
            minH = 10;

        var posX = x;
        var posY = y;
        var pad = (svgPadding * 2);

        var elmstyle = svgselection[0].style;
        var rectsvg =
            {
                left: pos2Float(elmstyle.left),
                top: pos2Float(elmstyle.top),
                width: pos2Float(elmstyle.width),
                height: pos2Float(elmstyle.height),
            };

        if (x != null && minW <= rectsvg.width + w - (pad * 2)) {
            svgselection.css({ "left": (rectsvg.left + x) + 'px' });
            //console.log('rectsvg x: ' + rectsvg.left + ' ' + x + ' = ' + (rectsvg.left+x));
        }
        if (y != null && minH <= rectsvg.height + h - (pad * 2)) {
            svgselection.css({ "top": (rectsvg.top + y) + 'px' });
        }

        if (w != null && minW <= rectsvg.width + w - (pad * 2)) {
            svgselection.css({ "width": (rectsvg.width + w) + 'px' });
            rectselection.css({ "width": (rectsvg.width + w - pad) + 'px' });
        }
        if (h != null && minH <= rectsvg.height + h - (pad * 2)) {
            svgselection.css({ "height": (rectsvg.height + h) + 'px' });
            rectselection.css({ "height": (rectsvg.height + h - pad) + 'px' });
            //console.log('rectsvg h: ' + rectsvg.height + ' ' + h + ' = ' + (rectsvg.height + h));
        }
        $scope.repositionSelectorPath();
        resizeElement(id);
    }
    var resizeElement = function (id) {
        var svgselection = $('#svg-selection');
        var elmstyle = svgselection[0].style;
        var rectsvg =
            {
                left: pos2Float(elmstyle.left),
                top: pos2Float(elmstyle.top),
                width: pos2Float(elmstyle.width),
                height: pos2Float(elmstyle.height),
            };

        var pad = (svgPadding * 4);
        var selected = $('#' + id);
        var parent = $('#' + selected[0].parentNode.id);

        parent.css({ "left": (rectsvg.left + svgPadding) + 'px' });
        parent.css({ "top": (rectsvg.top + svgPadding) + 'px' });
        parent.css({ "width": (rectsvg.width - (svgPadding * 2)) + 'px' });
        parent.css({ "height": (rectsvg.height - (svgPadding * 2)) + 'px' });

        selected.css({ "width": (rectsvg.width - pad) + 'px' });
        selected.css({ "height": (rectsvg.height - pad) + 'px' });

        var i = $scope.findAnnoItem(selected[0].parentNode.id);
        var item = $scope.annoItems[i];
        item.LeftPosition = rectsvg.left + (svgPadding * 2);
        item.TopPosition = rectsvg.top + (svgPadding * 2);;
        item.WidthPosition = rectsvg.width - pad;
        item.HeightPosition = rectsvg.height - pad;

        $scope.debugText = "L:" + item.LeftPosition + " T:" + item.TopPosition + " W:" + item.WidthPosition + " H:" + item.HeightPosition;
    }

    var delKeyDown = function(e)
    {
        if (!isAnnoElementEnable) return;
        if (event.keyCode == 46 && $('#svg-selection')[0].style.display != 'none') {
            swal({
                title: "Confirmation",
                text: "This annotate will be deleted, are you sure?",
                type: "warning",
                showCancelButton: true,
                closeOnConfirm: false,
                confirmButtonColor: "#2196F3",
                showLoaderOnConfirm: true,
                confirmButtonText: "Yes, delete it!",
                cancelButtonText: "No, cancel plx!",
            },
        function (isConfirm) {
            //setTimeout(function () {
            if (isConfirm) {
                $scope.removeAnnoItem(selectedNodeId);
                selectedNodeId = null;
                killSelection();
            }
            swal.close();
            //}, 500);

        });
        }
    }
    var selectorMouseDown = function (event) {
        isselector = true;
        selectedMouseDown = event.target.classList[1];
        isresize = true;
        var selected = $('#' + selectedNodeId);
        
        //console.log(selected[0].offsetHeight);
        
        var outc = recoverClientValues(event);
        
        lastMouseX = outc.x;
        lastMouseY = outc.y;
        
    }
    var selectorMouseMove = function (event) {
        if (!isresize) return;

        var outc = recoverClientValues(event);

        var currMouseX = outc.x;
        var currMouseY = outc.y;

        var deltaX = currMouseX - lastMouseX;
        var deltaY = currMouseY - lastMouseY;

        //this.applyMouseMoveAction(deltaX, deltaY);
        var outp = recoverPageValues(event);
        lastMouseX = outp.x;
        lastMouseY = outp.y;
        
        //var selected = $('#' + selectedNodeId);
        var selected = $('#svg-selection');
        //var selectedNodeId = 'svg-selection';
        if (selectedMouseDown == 'resize-cursor-topleft') {
            resizeSelector(selectedNodeId, -deltaX, -deltaY, deltaX, deltaY);
        } else if (selectedMouseDown == 'resize-cursor-topcenter') {
            resizeSelector(selectedNodeId, null, -deltaY, null, deltaY);
        } if (selectedMouseDown == 'resize-cursor-topright') {
            resizeSelector(selectedNodeId, deltaX, -deltaY, null, deltaY);
        } if (selectedMouseDown == 'resize-cursor-right') {
            resizeSelector(selectedNodeId, deltaX, null, null, null);
        } if (selectedMouseDown == 'resize-cursor-bottomright') {
            resizeSelector(selectedNodeId, deltaX, deltaY, null, null);
        } if (selectedMouseDown == 'resize-cursor-bottomcenter') {
            resizeSelector(selectedNodeId, null, deltaY, null, null);
        } if (selectedMouseDown == 'resize-cursor-bottomleft') {
            resizeSelector(selectedNodeId, -deltaX, deltaY, deltaX, null);
        } if (selectedMouseDown == 'resize-cursor-left') {
            resizeSelector(selectedNodeId, -deltaX, null, deltaX, null);
        }
        console.log(deltaX + ' ' + deltaY);

        //$scope.repositionSelector(selectedNodeId);
    }
    var selectorMouseUp = function (event) {
        isresize = false;
        isselector = false;
    }
    var documentMouseDown = function (event) {
        if (isselector || event.target.classList[0] == 'rect-selection') return;
        killSelection();
    }

    var killSelection = function () {
        $scope.annoSelectorRemoveEvent();
        $('.svg-selection').hide();
        isresize = false;
        isselector = false;
    }

    var createAnnoLayers = function (numPages) {
        for (i = 1; i <= numPages; i++) {
            createAnnoLayer(i);
        }
    }
    var createAnnoLayer = function (page) {
        //var layer = document.getElementById(annoLayerClass + page);
        //if (layer == null) {
        //    if (document.getElementById('page' + page) == null)
        //        return null;

        //    var canvas = $('#page' + page);
        //    var canvasParent = canvas.parent().parent();
        //    var textLayer = canvasParent;//.find('.textLayer');
        //    textLayer.append("<div id='" + annoLayerClass + page + "' class='" + annoLayerClass + "'></div>");
        //}
        return $('#' + annoLayerClass + page);
    }
    var enableAnnoLayer = function (isEnabled) {

        var event = 'stroke';
        if (!isEnabled)
            event = 'none';

        $('.' + annoLayerClass).css({ 'pointer-events': event });
    }

    var textMouseClick = function (e, page) {
        var item = angular.copy(annoItem);
        item.Page = page;
        item.AnnotateType = annotationType.TEXT;
        item.TopPosition = e.offsetY;
        item.LeftPosition = e.offsetX;
        item.ScaleX = 1;
        item.ScaleY = 1;
        $scope.addAnnoItem(item);
        
        $scope.clickObjectButtonPointer();
        $scope.addAnnoText(svgNo);
        var text = document.getElementById(editTextClass + svgNo);
        setTimeout(function () {
            text.focus();
        }, 0);
        svgNo++;
    };

    var itemArrange = function (dataIdx) {
        var item = $scope.annoItems[dataIdx];
        if (item.WidthPosition != null)
            $('#' + svgElement.id).css({ "width": item.WidthPosition + 'px' })
        if (item.HeightPosition != null)
            $('#' + svgElement.id).css({ "height": item.HeightPosition + 'px' });
        $('#' + svgElement.id).css({ 'top': item.TopPosition + 'px' });
        $('#' + svgElement.id).css({ 'left': item.LeftPosition + 'px' });
    }
    var signatureArrange = function (theClass, offsetX, offsetY) {
        var field = $('#' + svgElement.id).find('.' + theClass);
        var rect = {
            top: field[0].offsetTop,
            left: field[0].offsetLeft,
            width: field[0].offsetWidth,
            height: field[0].offsetHeight,
        };
        var valCenterH = (rect.height / 2);
        var valCenterW = (rect.width / 2);
        if (!dropedToCenter) {
            valCenterH = valCenterW = 0;
        }

        $('#' + svgElement.id).css({ "width": rect.width + (svgPadding * 2) + 'px' })
        $('#' + svgElement.id).css({ "height": rect.height + (svgPadding * 2) + 'px' });
        if (offsetY != null)
            $('#' + svgElement.id).css({ 'top': offsetY - svgPadding - valCenterH + 'px' });
        if (offsetX != null)
            $('#' + svgElement.id).css({ 'left': offsetX - svgPadding - valCenterW + 'px' });
        $('#' + svgElement.id).css({ 'pointer-events': 'none' });
    }
    var positionArrange = function (theClass, offsetX, offsetY) {
        var field = $('#' + svgElement.id).find('.' + theClass);
        //var rect = field[0].getClientRects();
        if (field[0].offsetTop == undefined) {
            rect = {
                top: field[0].clientTop,
                left: field[0].clientLeft,
                width: field[0].clientWidth,
                height: field[0].clientHeight,
            };
        } else {
            rect = {
                top: field[0].offsetTop,
                left: field[0].offsetLeft,
                width: field[0].offsetWidth,
                height: field[0].offsetHeight,
            };
        }

        $('#' + svgElement.id).css({ "width": rect.width + (svgPadding * 2) + 'px' })
        $('#' + svgElement.id).css({ "height": rect.height + (svgPadding * 2) + 'px' });
        if (offsetY != null)
            $('#' + svgElement.id).css({ 'top': offsetY + 'px' });
        if (offsetX != null)
            $('#' + svgElement.id).css({ 'left': offsetX + 'px' });
    }

    var signatureMouseClick = function (e, page) {
        var item = angular.copy(annoItem);
        item.Page = page;
        item.AnnotateType = annotationType.SIGNATURE;
        item.TopPosition = e.offsetY;
        item.LeftPosition = e.offsetX;
        item.ScaleX = 1;
        item.ScaleY = 1;
        $scope.addAnnoItem(item);
        $scope.clickObjectButtonPointer();
        $scope.addAnnoSignature(svgNo);
        $scope.annoSelector(signatureClass + '-' + svgNo);
        svgNo++;
    }
    var initialMouseClick = function (e, page) {
        var item = angular.copy(annoItem);
        item.Page = page;
        item.AnnotateType = annotationType.INITIAL;
        item.TopPosition = e.offsetY;
        item.LeftPosition = e.offsetX;
        item.ScaleX = 1;
        item.ScaleY = 1;
        $scope.addAnnoItem(item);
        $scope.clickObjectButtonPointer();
        $scope.addAnnoInitial(svgNo);
        $scope.annoSelector(initialClass + '-' + svgNo);
        svgNo++;
    }
    var privateStampMouseClick = function (e, page) {
        var item = angular.copy(annoItem);
        item.Page = page;
        item.AnnotateType = annotationType.PRIVATESTAMP;
        item.TopPosition = e.offsetY;
        item.LeftPosition = e.offsetX;
        item.ScaleX = 1;
        item.ScaleY = 1;
        $scope.addAnnoItem(item);
        $scope.clickObjectButtonPointer();
        $scope.addAnnoPrivateStamp(svgNo);
        $scope.annoSelector(privateStampClass + '-' + svgNo);
        svgNo++;
    }
    var stampMouseClick = function (e, page) {
        var item = angular.copy(annoItem);
        item.Page = page;
        item.AnnotateType = annotationType.STAMP;
        item.TopPosition = e.offsetY;
        item.LeftPosition = e.offsetX;
        item.ScaleX = 1;
        item.ScaleY = 1;
        $scope.addAnnoItem(item);
        $scope.clickObjectButtonPointer();
        $scope.addAnnoStamp(svgNo);
        $scope.annoSelector(stampClass + svgNo);
        //childNo++;
        svgNo++;
    }
    var onEditText = function (e) {
        var svgId = e.target.parentNode.id;
        var text = e.target.innerText;
        var iItem = $scope.findAnnoItem(svgId);
        if (iItem != -1)
            $scope.annoItems[iItem].Data = text;

        var w = e.path[0].clientWidth;
        var h = e.path[0].clientHeight;

        $('#' + e.path[1].id).css({ "width": (w + 10) + 'px' })
        $('#' + e.path[1].id).css({ "height": h + 'px' });

        $scope.annoSelectorRemoveEvent();
        $('.svg-selection').hide();
        isresize = false;
    }
    var selectionDblClick = function (e) {
        if (!isAnnoElementEnable) return;
        if (selectedNodeId.startsWith(editTextClass)) {
            $scope.annoSelectorRemoveEvent();
            $('.svg-selection').hide();
            isresize = false;
            $('#' + selectedNodeId).focus();
        } else if (selectedNodeId.startsWith(signatureClass)) {
            $scope.popupMember(selectedNodeId);
        } else if (selectedNodeId.startsWith(initialClass)) {
            $scope.popupMember(selectedNodeId);
        } else if (selectedNodeId.startsWith(privateStampClass)) {
            $scope.popupMember(selectedNodeId);
        } else if (selectedNodeId.startsWith(stampClass)) {
            $scope.popupStamp(selectedNodeId);
        }

    }


    var isTouchEvent = function (e) {
        return e.type.match(/^touch/);
    }

    var pointerEventToXY = function (e) {
        var out = { x: 0, y: 0 };
        if (isTouchEvent(e)) {
            var touch = e.originalEvent.touches[0] || e.originalEvent.changedTouches[0];
            out.x = touch.pageX;
            out.y = touch.pageY;
        } else {
            out.x = e.pageX;
            out.y = e.pageY;
        }
        return out;
    };

    function recoverOffsetValues(e) {
        if (isTouchEvent(e)) {
            var rect = e.target.getBoundingClientRect();
            var bodyRect = document.body.getBoundingClientRect();
            //var x = e.originalEvent.changedTouches[0].pageX - (rect.left - bodyRect.left);
            //var y = e.originalEvent.changedTouches[0].pageY - (rect.top - bodyRect.top);
            ////var x = e.changedTouches[0].pageX - (rect.left);// - bodyRect.left);
            ////var y = e.changedTouches[0].pageY - (rect.top);// - bodyRect.top);


            var x = e.targetTouches[0].clientX - (rect.left);// - bodyRect.left);
            var y = e.targetTouches[0].clientY - (rect.top);// - bodyRect.top);
            var out = { x: x, y: y };
            return out;
        } else {
            //var out = { x: e.offsetX, y: e.offsetY };
            //var x = e.clientX;
            //var y = e.clientY;
            var out = { x: e.offsetX, y: e.offsetY };
            return out;
        }
    }

    function recoverClientValues(e) {
        if (isTouchEvent(e)) {
            var x = e.targetTouches[0].clientX;
            var y = e.targetTouches[0].clientY;

            var out = { x: x, y: y };
            return out;
        } else {
            var out = { x: e.clientX, y: e.clientY };
            return out;
        }
    }

    function recoverPageValues(e) {
        if (isTouchEvent(e)) {
            var x = e.targetTouches[0].pageX;
            var y = e.targetTouches[0].pageY;
            var out = { x: x, y: y };
            return out;
        } else {
            var out = { x: e.pageX, y: e.pageY };
            return out;
        }
    }

    var penMouseDown = function (e, page) {
        var annolayer = createAnnoLayer(page);
        var svg = '<svg id="svg' + svgNo + '" class="svg" style="position:absolute; top:0px; left:0px;width:100%;height:100%"></svg>';
        annolayer.append(svg);
        svgElement = document.getElementById("svg" + svgNo);

        //svgElement.addEventListener("mousemove", penMouseMove);
        //svgElement.addEventListener("mouseup", penMouseUp);
        //svgElement.addEventListener("touchmove", penMouseMove);
        //svgElement.addEventListener("touchend", penMouseUp);

        document.addEventListener("mousemove", penMouseMove);
        document.addEventListener("mouseup", penMouseUp);
        document.addEventListener("touchmove", penMouseMove);
        document.addEventListener("touchend", penMouseUp);

        path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
        path.setAttribute("fill", "none");
        path.setAttribute("stroke", colorDefault);
        path.setAttribute("stroke-width", strokeWidth);
        path.setAttribute("style", "cursor:pointer;pointer-events:stroke;opacity:" + opacityColor);

        buffer = [];
        var pt = {};
        var out = recoverOffsetValues(e);
        pt.x = out.x;//e.offsetX;
        pt.y = out.y;//e.offsetY;
        appendToBuffer(pt);
        strPath = "M" + pt.x + " " + pt.y;
        path.setAttribute("d", strPath);
        svgElement.appendChild(path);

        tmpPenAnnoItem.SvgId = "svg" + svgNo;
        tmpPenAnnoItem.Page = page;
        tmpPenAnnoItem.Color = colorDefault;
        //tmpPenAnnoItem.BackColor = backColor;
        //tmpPenAnnoItem.Data = data;
        //tmpPenAnnoItem.ScaleX = scale;
        //tmpPenAnnoItem.ScaleY = scale;
        tmpPenAnnoItem.StrokeWidth = strokeWidth;
        tmpPenAnnoItem.Opacity = opacityColor;

        if (isTouchEvent(e)) e.preventDefault();
    };
    var penMouseMove = function (e) {
        if (toolType == annotationType.POINTER)
            return;
        if (path) {
            var pt = {};
            var out = recoverOffsetValues(e);

            pt.x = out.x;
            pt.y = out.y;
            appendToBuffer(pt);
            updateSvgPath();
        }

        if (isTouchEvent(e)) e.preventDefault();
    };
    var penMouseUp = function () {
        enableAnnoLayer(false);

        //svgElement.removeEventListener("mousemove", penMouseMove);
        //svgElement.removeEventListener("mouseup", penMouseUp);
        //svgElement.removeEventListener("touchmove", penMouseMove);
        //svgElement.removeEventListener("touchend", penMouseUp);

        document.removeEventListener("mousemove", penMouseMove);
        document.removeEventListener("mouseup", penMouseUp);
        document.removeEventListener("touchmove", penMouseMove);
        document.removeEventListener("touchend", penMouseUp);

        if (toolType == annotationType.POINTER)
            return;
        $scope.clickObjectButtonPointer();
        if (path) {
            var rc = svgElement.getBoundingClientRect();
            var rect = path.getBoundingClientRect();
            var pad = svgPadding * 2;

            var d = path.attributes['d'].value;
            var ds = d.split(' ');
            var newd = "";
            var xy = 0;
            for (i = 0; i < ds.length; i++) {
                var digit1 = "";
                var value = 0;
                if ($.isNumeric(ds[i]))
                    value = parseFloat(ds[i]);
                else {
                    digit1 = ds[i].substring(0, 1);
                    value = parseFloat(ds[i].substring(1));
                }

                data = digit1 + ((xy != 0 ? value - (rect.top - rc.top) : value - (rect.left - rc.left)) + pad) + " ";
                newd += data;
                if (xy == 0) xy = 1; else xy = 0;
            }
            newd += "XX";
            newd = newd.replace(' XX', '');
            path.setAttribute('d', newd);
            path.setAttribute('id', penClass + svgNo);
            path.setAttribute('class', penClass);
            path.setAttribute('onclick', "annoSelector(" + "'" + penClass + svgNo + "'" + ")");

            var t = rect.top - rc.top - pad;
            var l = rect.left - rc.left - pad;
            var w = rect.width + (pad * 2);
            var h = rect.height + (pad * 2);

            $('#' + svgElement.id).css({ 'top': t + 'px' });
            $('#' + svgElement.id).css({ 'left': l + 'px' });
            $('#' + svgElement.id).css({ 'width': w + 'px' });
            $('#' + svgElement.id).css({ 'height': h + 'px' });
            $('#' + svgElement.id).css({ 'pointer-events': 'none' });
            $('#' + svgElement.id)[0].setAttribute('preserveAspectRatio', "none");
            $('#' + svgElement.id)[0].setAttribute('viewBox', '0 0 ' + w + ' ' + h);

            var item = angular.copy(annoItem);
            item.Page = tmpPenAnnoItem.Page;
            item.AnnotateType = annotationType.PEN;
            item.TopPosition = t;
            item.LeftPosition = l;
            item.WidthPosition = w;
            item.HeightPosition = h;
            item.Color = tmpPenAnnoItem.Color;
            item.BackColor = tmpPenAnnoItem.BackColor;
            item.Data = newd;
            item.Data2 = '0 0 ' + w + ' ' + h;
            item.ScaleX = 1;
            item.ScaleY = 1;
            item.StrokeWidth = tmpPenAnnoItem.StrokeWidth;
            item.Opacity = tmpPenAnnoItem.Opacity;
            $scope.addAnnoItem(item);
            //childNo++;
            svgNo++;
            path = null;
        }

        if (isTouchEvent(e)) e.preventDefault();
    };

    var getMousePosition = function (e) {
        rect = svgElement.getBoundingClientRect();
        return {
            x: e.pageX - rect.left,
            y: e.pageY - rect.top
        }
    };

    var appendToBuffer = function (pt) {
        buffer.push(pt);
        while (buffer.length > bufferSize) {
            buffer.shift();
        }
    };

    // Calculate the average point, starting at offset in the buffer
    var getAveragePoint = function (offset) {
        var len = buffer.length;
        if (len % 2 === 1 || len >= bufferSize) {
            var totalX = 0;
            var totalY = 0;
            var pt, i;
            var count = 0;
            for (i = offset; i < len; i++) {
                count++;
                pt = buffer[i];
                totalX += pt.x;
                totalY += pt.y;
            }
            return {
                x: totalX / count,
                y: totalY / count
            }
        }
        return null;
    };

    var updateSvgPath = function () {
        var pt = getAveragePoint(0);

        if (pt) {
            // Get the smoothed part of the path that will not change
            var tmpPath = "";
            if (!lineStraight) {
                strPath += " L" + pt.x + " " + pt.y;
                //// Get the last part of the path (close to the current mouse position)
                //// This part will change if the mouse moves again
                //for (var offset = 2; offset < buffer.length; offset += 2) {
                //    pt = getAveragePoint(offset);
                //    tmpPath += " L" + pt.x + " " + pt.y;
                //}
            } else {
                tmpPath = " L" + pt.x + " " + pt.y;
            }
            // Set the complete current path coordinates
            path.setAttribute("d", strPath + tmpPath);
        }
    };

    $scope.finishLoading = function () {
        var a = pdfjsLib.AnnotationLayer;
    }
    
    $scope.pdfRender = function (page, parent) {
        var trf = $scope.transform;
        var trftext = 'rotate(' + trf.textAbsRotation + 'deg) ' + 'scale(' + trf.scaleX + ', ' + trf.scaleY + ') ';// + 'translate(' + trf.transX + ', ' + trf.transY + ')';

        var annoLayerDiv = document.createElement('div');
        annoLayerDiv.className = annoLayerClass;// + ' annotationLayer';
        annoLayerDiv.setAttribute('page-no', page);
        annoLayerDiv.id = annoLayerClass + page;
        annoLayerDiv.style.width = parent.lastElementChild.style.width;
        annoLayerDiv.style.height = parent.lastElementChild.style.height;
        //annoLayerDiv.style.transform = trftext;
        //annoLayerDiv.style.transformOrigin = '0% 0%';
        parent.appendChild(annoLayerDiv);
        $scope.pdfSetScale(1);
        $scope.bindAnnotation(page);
    }

    $scope.pdfTransform = function (page, textAbsRotation, scaleX, scaleY, transX, transY) {
        $scope.transform.textAbsRotation = textAbsRotation;
        $scope.transform.scaleX = scaleX;
        $scope.transform.scaleY = scaleY;
        $scope.transform.transX = transX;
        $scope.transform.transY = transY;
        //alert('page:' + page + ' textAbsRotation:' + textAbsRotation + ' scaleX:' + scaleX + ' scaleY:' + scaleY + ' transX:' + transX + ' transY:' + transY);
    }

    $scope.clickRotate = function () {
        var a = pdfjsLib.AnnotationLayer;
    }
    function dragElement(elmnt) {
        var pos1 = 0, pos2 = 0, pos3 = 0, pos4 = 0;
        if (document.getElementById(elmnt.id)) {
            // if present, the header is where you move the DIV from:
            document.getElementById(elmnt.id).onmousedown = dragMouseDown;
            document.getElementById(elmnt.id).addEventListener("touchstart", dragMouseDown);
        } else {
            //otherwise, move the DIV from anywhere inside the DIV:
            elmnt.onmousedown = dragMouseDown;
            elmnt.ontouchstart.addEventListener("touchstart", dragMouseDown);
        }

        function dragMouseDown(e) {
            e = e || window.event;
            e.preventDefault();
            // get the mouse cursor position at startup:
            var out = recoverClientValues(e);
            pos3 = out.x;
            pos4 = out.y;
            //console.log('dragMouseDown clientX: ' + e.clientY + ', clientY: ' + e.clientY);
            document.onmouseup = closeDragElement;
            document.addEventListener("touchend", closeDragElement);
            // call a function whenever the cursor moves:
            document.onmousemove = elementDrag;
            document.addEventListener("touchmove", elementDrag);
        }

        function elementDrag(e) {
            
            e = e || window.event;
            e.preventDefault();
            // calculate the new cursor position:
            var out = recoverClientValues(e);
            pos1 = pos3 - out.x;
            pos2 = pos4 - out.y;
            pos3 = out.x;
            pos4 = out.y;
            //alert(out.x + " " + out.y);
            //console.log(selectedNodeId+' | elementDrag clientX: ' + e.clientY + ', clientY: ' + e.clientY);
            // set the element's new position:
            elmnt.style.top = (pos2Float(elmnt.style.top) - pos2) + "px";
            elmnt.style.left = (pos2Float(elmnt.style.left) - pos1) + "px";
            console.log(selectedNodeId + ' | elementDrag TOP: ' + (elmnt.offsetTop - pos2) + ', LEFT: ' + (elmnt.offsetLeft - pos1));

            var svg = document.getElementById("svg-selection");
            //var selNode = document.getElementById(selectedNodeId);
            //var svg = document.getElementById(selNode.parentNode.id);
            svg.style.top = (pos2Float(svg.style.top) - pos2) + "px";
            svg.style.left = (pos2Float(svg.style.left) - pos1) + "px";

            resizeElement(selectedNodeId);
        }

        function closeDragElement() {
            // stop moving when mouse button is released:
            document.onmouseup = null;
            document.onmousemove = null;

            document.removeEventListener("touchmove", elementDrag);
            document.removeEventListener("touchend", closeDragElement);
        }
    }

    // export function
    $scope.getAnnoItems = function () {
        return $scope.annoItems;
    }
    $scope.setAnnoItems = function (items) {
        $scope.annoItems = items;
        svgNo = items.length;
    }

    var isAnnoToolbarVisible = true;    // true: toolbar visible, anno editable | false: toolbar hide, anno not editable
    //var isAnnoElementVisible = true;
    var isAnnoElementEnable = true;
    $scope.setAnnoToolbarVisible = function (flag) {
        isAnnoToolbarVisible = flag;
        if (isAnnoToolbarVisible) {
            $('#xannoToolbar').show();
            //$('#toolbarContainer').css({ 'height': '64px' });
            //$('#viewerContainer').css({ 'top': '64px' });
            isAnnoElementEnable = true;
        } else {
            $('#xannoToolbar').hide();
            //$('#toolbarContainer').css({ 'height': '32px' });
            //$('#viewerContainer').css({ 'top': '32px' });
            isAnnoElementEnable = false;
        }
        //var ax = PDFJS.PDFViewer();
        //var ac=PDFJS.PDFPageView();
    }
    //$scope.setAnnoElementVisible = function (flag) {
    //    isAnnoElementVisible = flag;
    //}
    $scope.setAnnoElementEnable = function (flag) {
        isAnnoElementEnable = flag;
    }

    $scope.setDefaultPdf = function (filename) {
        $scope.defaultDocumentName=filename;
        PDFViewerApplication.open('/doc/mgn/' + filename);
    }
    $scope.showPrint = function (condition) {
        if (condition == undefined || condition == true) {
            $('#print').show();
            $('#secondaryPrint').show();
        }
    }
    $scope.showDownload = function (condition) {
        if (condition == undefined || condition == true) {
            $('#download').show();
            $('#secondaryDownload').show();
        }
    }
    $scope.pdfSetScale = function (scale) {
        //PDFViewerApplication.toolbar.setPageScale(percent, scale);
        //PDFViewerApplication.pdfViewer.update();
        PDFViewerApplication.pdfViewer.currentScale = scale;
    }
    // end export function

    /*--------------------------------------------------------------
                POPUP MEMBER
    --------------------------------------------------------------*/
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

    $scope.popupMember = function (id) {
        //$scope.members = [];
        $("#modal_select_member").modal("show");
    }

    $scope.setMember = function (idx) {
        var parent = $('#' + selectedNodeId)[0].parentElement.id;
        var i = $scope.findAnnoItem(parent);
        var item = $scope.annoItems[i];
        item.ElementId = $scope.members[idx].Id;
        item.Annotate.Number = $scope.members[idx].Number;
        item.Annotate.Name = $scope.members[idx].Name;
        item.Annotate.Foto = $scope.members[idx].ImageProfile;

        var no = parent.replace('svg','');
        var foto = document.getElementById("member-foto-" + no);
        foto.src = "/Images/Member/" + item.Annotate.Foto;
        $("#member-name-" + no).text(item.Annotate.Name + ' | ' + item.Annotate.Number);
    }

    $scope.getLiteMembers = function (kriteria, page, row) {
        $scope.members = [];
        $http.post('/Member/GetLiteGroupAll', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.members = response.data;

                $scope.index = row * (page - 1);

                $scope.paging = [];
                $scope.getMemberCount(kriteria);

                $scope.isView = true;
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }

    $scope.changePageMember = function (kriteria, page, row) {
        $scope.members = [];
        $http.post('/Member/GetLiteGroupAll', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.members = response.data;
                $scope.index = row * (page - 1);
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }

    $scope.getMemberCount = function (kriteria) {
        $http.post('/Member/GetLiteGroupAllCount', { topCriteria: kriteria }).then(function (response) {
            if (response.data) {

                var jumlahData = response.data;
                var jumlahPage = Math.ceil(jumlahData / $scope.row);
                for (var i = 1; i <= jumlahPage; i++) {
                    $scope.paging.push({ value: i, text: i });
                }

                $scope.page = "1";

            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }


    /*--------------------------------------------------------------
        END POPUP MEMBER
    --------------------------------------------------------------*/

    /*--------------------------------------------------------------
        POPUP STAMP
    --------------------------------------------------------------*/
    // modal stamp
    $scope.stamp = {};
    $scope.stamps = [];
    $scope.stampCount = [];
    $scope.st_paging = [];
    $scope.st_kriteria = "";
    $scope.st_page = 1;
    //$scope.row = 20;
    $scope.st_currPage = 0;
    $scope.st_index = 0;
    // modal stamp

    $scope.popupStamp = function (id) {
        //$scope.stamps = [];
        $("#modal_select_stamp").modal("show");
    }

    $scope.setStamp = function (idx) {
        var parent = $('#' + selectedNodeId)[0].parentElement.id;
        var i = $scope.findAnnoItem(parent);
        var item = $scope.annoItems[i];
        item.ElementId = $scope.stamps[idx].Id;
        item.Annotate.Name = $scope.stamps[idx].Descr;
        item.Annotate.Foto = $scope.stamps[idx].StampFile;

        var foto = document.getElementById(selectedNodeId);
        foto.src = "/Images/Stamp/" + item.Annotate.Foto;
    }

    $scope.getLiteStamps = function (kriteria, page, row) {
        $scope.stamps = [];
        $http.post('/Stamp/GetLiteAll', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.stamps = response.data;

                $scope.index = row * (page - 1);

                $scope.st_paging = [];
                $scope.getStampCount(kriteria);

                $scope.isView = true;
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }

    $scope.changePageStamp = function (kriteria, page, row) {
        $scope.stamps = [];
        $http.post('/Stamp/GetLiteAll', { topCriteria: kriteria, page: page, pageSize: row }).then(function (response) {
            if (response.data) {
                $scope.stamps = response.data;
                $scope.st_index = row * (page - 1);
            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }

    $scope.getStampCount = function (kriteria) {
        $http.post('/Stamp/GetLiteAllCount', { topCriteria: kriteria }).then(function (response) {
            if (response.data) {

                var jumlahData = response.data;
                var jumlahPage = Math.ceil(jumlahData / $scope.row);
                for (var i = 1; i <= jumlahPage; i++) {
                    $scope.st_paging.push({ value: i, text: i });
                }

                $scope.st_page = "1";

            }
        }, function (response) {
            //error handle\
            var x = 0;
        });
    }


    /*--------------------------------------------------------------
        END POPUP STAMP
    --------------------------------------------------------------*/
});

function annoSelector(idx) {
    var scope = angular.element(document.getElementById("xpdfController")).scope();
    scope.$apply(function () {
        scope.annoSelector(idx);
    });
}
function pdfRender(page, parent) {
    var scope = angular.element(document.getElementById("xpdfController")).scope();
    scope.$apply(function () {
        //alert('render');
        scope.pdfRender(page, parent);
    });
}
function pdfTransform(page, textAbsRotation, scaleX, scaleY, transX, transY) {
    var scope = angular.element(document.getElementById("xpdfController")).scope();
    scope.$apply(function () {
        //alert('pdfTransform');
        scope.pdfTransform(page, textAbsRotation, scaleX, scaleY, transX, transY);
    });
}