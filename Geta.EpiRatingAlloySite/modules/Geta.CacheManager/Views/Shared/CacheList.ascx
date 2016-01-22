<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Geta.CacheManager.Core.Models.CacheListModel>" %>
<%@ Assembly Name="Geta.CacheManager" %>
<%@ Assembly Name="Geta.CacheManager.Core" %>
<%@ Import Namespace="Geta.CacheManager" %>
<%@ Import Namespace="Geta.CacheManager.Core" %>
<%@ Import Namespace="Geta.CacheManager.Core.Models" %>

<div id="advanced">
    <h1 class="EP-prefix">Delete Cache Entries</h1>

    <div>Please select the cache entries you wish to delete, and click the button below to delete.</div>

    <div class="epi-buttonDefault">
        <span class="epi-cmsButton">
            <input type="button" id="btnDeleteSelected" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Delete" value="Delete Selected">
        </span>
        <input id="tblFilter" type="text" placeholder="Filter current page" />
    </div>

    <table id="tblCacheList" class="epi-default">
        <thead>
            <tr>
                <th class="tiny"><input type="checkbox" class="select-all"></th>
                <th class="sortKey" data-sortKey="Key"><span>Key</span></th>
                <th class="sortKey" data-sortKey="Value"><span>Value</span></th>
                <th class="sortKey" data-sortKey="EntryType"><span>Type</span></th>
                <th class="sortKey active" data-sortKey="Size"><span>Size</span></th>
                <th class="sortKey" data-sortKey="Created"><span>Created</span></th>
                <th class="sortKey" data-sortKey="Expires"><span>Expires</span></th>
            </tr>
        </thead>
        <tbody>
            <% foreach (CacheModel cacheItem in Model.CacheList) { %>
                <tr>
                    <td><input type="checkbox" value="<%= cacheItem.Key %>"/></td>
                    <td><%= cacheItem.Key %></td>
                    <td><%= cacheItem.Value %></td>
                    <td><%= cacheItem.EntryType %></td>
                    <td><%= cacheItem.SizeFormatted %></td>
                    <td><%= cacheItem.CreatedFormatted %></td>
                    <td><%= cacheItem.ExpiresFormatted %></td>
                </tr>
            <% } %>
        </tbody>
    </table>
    <div id="cacheListPager" class="epi-buttonDefault">
        <span class="epi-cmsButton">
            <input type="button" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-ArrowLeft prev-page" value="">
        </span>
        <span>
            <input type="text" class="page-number" value="<%= Model.PageNumber %>"/> of <span class="total-pages"><%= Model.TotalPages %></span>
        </span>
        <span class="epi-cmsButton">
            <input type="button" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-ArrowRight next-page" value="">
        </span>
        <span class="page-size-wrapper">
            Items per page:
            <select class="page-size">
                <option value="10">10</option>
                <option value="30">30</option>
                <option value="50">50</option>
                <option value="100">100</option>
            </select>
        </span>
    </div>
</div>