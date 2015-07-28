<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuilderClass.aspx.cs" Inherits="Demo.BuilderClass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>生成实体</title>
    <style type="text/css">
        #txtResult
        {
            height: 114px;
            width: 683px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        -----------------------------------------根据数据库生成实体文件---------------------------<br />
        命名空间:<asp:TextBox ID="txtNS" runat="server"></asp:TextBox>
        &nbsp;<br />
        物理路径 :
        <asp:TextBox ID="txtPath" runat="server" Width="480px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnCreate" runat="server" Text="生成" OnClick="btnCreate_Click" />
        <br />
        <br />
        <br />
        <br />
        -----------------------------------------根据SQL生成实体字符串---------------------------<br />
        <br />
        类名：<br />
        <asp:TextBox ID="txtClassName" runat="server"></asp:TextBox>
        <br />
        SQL:<br />
        <asp:TextBox ID="txtSql" runat="server" Height="74px" Width="366px"></asp:TextBox>
        <br />
        <br />
        <asp:Button ID="btnCreateClassCode" runat="server" Text="生成" OnClick="btnCreateClassCode_Click" />
        <br />
        <br />
        <asp:Label ID="lblResult" runat="server" Text="结果："></asp:Label>
        <br />
        <br />
        <textarea runat="server" id="txtResult"></textarea>
    </div>
    </form>
</body>
</html>
