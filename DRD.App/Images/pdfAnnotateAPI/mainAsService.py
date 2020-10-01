import uvicorn
import base64
import io
import os
import uuid

from typing import List, Optional
from fastapi import FastAPI, File
from pydantic import BaseModel
from pdf_annotate import PdfAnnotator, Location, Appearance
from pdfrw import PdfReader

import win32serviceutil
import win32service
import win32event
import servicemanager
from multiprocessing import Process
import socket

handler = FastAPI()

class IdentityDetails(BaseModel):
    name:str
    code:str

class AnnotationDetail(BaseModel):
    anType:str
    x:float
    y:float
    width:float
    height:float
    page:int
    identification:Optional[IdentityDetails] = None

class Item(BaseModel):
    pdffile:str
    annotations:List[AnnotationDetail]

@handler.get("/")
async def root():
    return {"message":"Hello World"}

@handler.post("/makepdf/")
async def makePdf(Item:Item):
    pdfBytes = base64.b64decode(Item.pdffile)
    reader = PdfReader(fdata=pdfBytes)
    annotator = PdfAnnotator(reader)
    for i in Item.annotations:
        if (i.anType == "line"):
            await addLine(annotator, i)
        elif (i.anType == 'highlighter'):
            await addHighlight(annotator, i)
        elif (i.anType == 'identity'):
            await addIdentity(annotator, i)
    fileid = uuid.uuid4()
    fileName = str(fileid)+".pdf"
    annotator.write(fileName)
    encoded_string = ""
    with open(fileName, "rb") as pdf_file:
        encoded_string = base64.b64encode(pdf_file.read())
    os.remove(fileName)
    return {"result":encoded_string}

async def addLine(pdf, annotationItem:AnnotationDetail):
    sizePapper = pdf.get_size(annotationItem.page-1)
    xStart = annotationItem.x
    yStart = sizePapper[1]-(annotationItem.y)
    xEnd = xStart + annotationItem.width
    yEnd = yStart - annotationItem.height
    pdf.add_annotation(
        'line',
        Location(points=[[xStart, yStart], [xEnd, yEnd]], page=annotationItem.page-1),
        Appearance(stroke_color=(1, 0, 0), stroke_width=4),
    )

async def addHighlight(pdf, annotationItem:AnnotationDetail):
    sizePapper = pdf.get_size(annotationItem.page-1)
    xStart = annotationItem.x
    yStart = sizePapper[1]-(annotationItem.y)
    xEnd = xStart + annotationItem.width
    yEnd = yStart - annotationItem.height
    pdf.add_annotation(
        'line',
        Location(points=[[xStart, yStart], [xEnd, yEnd]], page=annotationItem.page-1),
        Appearance(stroke_color=(1,1,0, 0.4), stroke_width=12),
    )

async def addIdentity(pdf, annotationItem:AnnotationDetail):
    sizePapper = pdf.get_size(annotationItem.page-1)
    xStart = annotationItem.x
    yStart = sizePapper[1]-(annotationItem.y)
    xEnd = xStart + annotationItem.width
    yEnd = yStart - annotationItem.height
    if annotationItem.identification is None:
        return
    textIdentification = annotationItem.identification.name + "\n" + annotationItem.identification.code
    pdf.add_annotation(
        'text',
        Location(x1=xStart, y1=yStart, x2=xEnd, y2=yEnd, page=annotationItem.page-1),
        Appearance(fill=[0.4, 0, 0.5], stroke_width=2, font_size=8, content=textIdentification),
    )

class Service(win32serviceutil.ServiceFramework):
    _svc_name_ = "PythonPDFAnnotateService"
    _svc_display_name_ = "Py PDF Annotate"
    _svc_description_ = "running on localhost port 8000 pdf annotator from python as API"

    
    def __init__(self,args):
        win32serviceutil.ServiceFramework.__init__(self,args)
        self.hWaitStop = win32event.CreateEvent(None,0,0,None)
        socket.setdefaulttimeout(60)
    
    def SvcStop(self):
        self.ReportServiceStatus(win32service.SERVICE_STOP_PENDING)
        win32event.SetEvent(self.hWaitStop)

    def SvcDoRun(self):
        servicemanager.LogMsg(servicemanager.EVENTLOG_INFORMATION_TYPE,
                              servicemanager.PYS_SERVICE_STARTED,
                              (self._svc_name_,''))
        self.main()

    def main(self):
        uvicorn.run(handler, host="127.0.0.1", port = 8000)

if __name__ == "__main__":
    win32serviceutil.HandleCommandLine(Service)
