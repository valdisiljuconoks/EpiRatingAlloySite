<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<Geta.CacheManager.Models.DefaultViewModel>" MasterPageFile="../Shared/Site.Master" Title="CacheManager" %>
<%@ Assembly Name="Geta.CacheManager" %>
<%@ Assembly Name="Geta.CacheManager.Core" %>
<%@ Import Namespace="Geta.CacheManager" %>
<%@ Import Namespace="Geta.CacheManager.Models" %>
<%@ Import Namespace="EPiServer.Shell" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">
	<div class="epi-padding-large">
		<% Html.RenderPartial("SystemStats", Model.SystemStats); %>
		<% Html.RenderPartial("CacheList", Model.CacheList); %>
	</div>
	
	<div id="ajaxOverlay">
		<div class="backgroundOverlay"></div>
		<img src="<%= Paths.ToShellClientResource("ClientResources/dojox/widget/Standby/images/loading.gif") %>" alt="Please wait..." class="loadingIcon"/>
	</div>
</asp:Content>