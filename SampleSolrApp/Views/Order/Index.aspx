<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<IEnumerable<Order>>" %>
<%@ Import Namespace="SampleSolrApp.Helpers"%>
<%@ Import Namespace="SampleSolrApp.Models"%>
<%@ Import Namespace="SampleSolrApp.Models.Nh"%>

<asp:Content ID="indexHead" ContentPlaceHolderID="head" runat="server">
    <title>Welcome to Megastore! 'Mega' means 'good', 'store' means 'thing'.</title>
</asp:Content>

<asp:Content ID="indexContent" ContentPlaceHolderID="MainContent" runat="server">
    

    <div class="rightColumn">

        <a href="/order/add">Dodaj Order</a>

        <% foreach (var order in Model) %>
        <% { %>
        <div>
            <%= order.Id %> <%= order.Name %> <%= order.Amount %> <hr />
         </div>
        <% } %>
    

    </div>
</asp:Content>
