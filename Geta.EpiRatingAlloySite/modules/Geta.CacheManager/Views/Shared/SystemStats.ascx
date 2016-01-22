<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Geta.CacheManager.Core.Models.SystemStatsModel>" %>
<%@ Assembly Name="Geta.CacheManager" %>
<%@ Assembly Name="Geta.CacheManager.Core" %>
<%@ Import Namespace="Geta.CacheManager" %>
<%@ Import Namespace="Geta.CacheManager.Core" %>

<div>
    <h1 class="EP-prefix">System Stats</h1>

    <div class="epi-contentContainer">
        <table class="epi-default">
            <tbody>
                <tr>
                    <td>Server</td>
                    <td id="cm-machine-name"><%= Model.MachineName %></td>
                </tr>
                <tr>
                    <td>Memory Usage</td>
                    <td id="cm-system-memory">
                        <div class="memory-usage-bar <%= Model.SystemMemory.UsedPercentage > 90 ? "critical" : (Model.SystemMemory.UsedPercentage > 75 ? "warning" : string.Empty) %>">
                            <div class="memory-usage-text">
                                <%= Model.SystemMemory.UsedFormatted %> of <%= Model.SystemMemory.TotalFormatted %>
                            </div>
                            <div class="memory-usage-bar-inner" style="width: <%= Model.SystemMemory.UsedPercentage.ToString() %>%"></div>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>Application Path</td>
                    <td id="cm-app-path"><%= Model.ApplicationPath %></td>
                </tr>
                <tr>
                    <td>Application Cache Entry Count</td>
                    <td id="cm-app-cache-count"><%= Model.AppCacheCount %></td>
                </tr>
            </tbody>
        </table>
    </div>

    <div class="epi-buttonDefault">
        <span class="epi-cmsButton">
            <input type="button" id="btnRefresh" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Refresh" value="Refresh"/>
        </span>
        <span class="epi-cmsButton">
            <input type="button" id="btnClearCache" class="epi-cmsButton-text epi-cmsButton-tools epi-cmsButton-Delete" value="Clear Cache"/>
        </span>
    </div>
</div>