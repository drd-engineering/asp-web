<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm1.aspx.cs" Inherits="DRD.App.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>


    <style type="text/css">
        .footer-user {
            width: 281px;
            height: 32px;
        }
        .default-user {
            width: 281px;
            height: 46px;
        }
        .user-selected {
            height: 46px;
        }
        .list-user {
            width: 281px;
            height: 36px;
        }
        .list-chat {
            height: 36px;
        }
        .textbox-chat {
            height: 32px;
        }
    </style>


</head>
<body>








    <table style="width:100%;">
        <tr>
            <td class="default-user" style="background-color: #C0C0C0">default user</td>
            <td class="user-selected" style="background-color: #C0C0C0">user selected</td>
        </tr>
        <tr>
            <td class="list-user">list user</td>
            <td class="list-chat" style="background-color: #33CCCC">list chat</td>
        </tr>
        <tr>
            <td class="footer-user">footer user</td>
            <td class="textbox-chat">textbox chat</td>
        </tr>
    </table>








</body>
</html>



