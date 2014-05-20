<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fn="http://www.w3.org/2005/xpath-functions">
  <xsl:output method="html"/>
  <xsl:strip-space elements="*"/>

  <xsl:template match="ApiCheckResult">
    <html>
      <head>
        <style type="text/css">
          body { margin:0; padding:0; font-family: sans-serif; }
          table { border-collapse:collapse; margin: 15px; }
          table tr td, table tr th { border: 1px solid black; }
          td, th { padding: 2px; }
          div { border: 1px solid black; margin: 15px; padding: 5px; background-color: #EEEEEE; }
          div h1 { font-size: large; margin: 0; }
          div h2 { font-size: medium; margin: 0; }
        </style>
      </head>
      <body>
        <h1 style="font-size: x-large; text-decoration: underline;">ApiCheck - Report</h1>
        <xsl:apply-templates select="ChangedElement">
          <xsl:sort select="@Name"/>
        </xsl:apply-templates>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="ChangedElement">
    <div>
      <h1>
        <xsl:value-of select="@Context"/> - <xsl:value-of select="@Name"/>
      </h1>

      <xsl:if test="ChangedAttribute">
        <h2>Changed Attributes:</h2>
        <table>
          <tr>
            <th>Name</th>
            <th>Reference Value</th>
            <th>New Value</th>
            <th>Severity</th>
          </tr>
          <xsl:apply-templates select="ChangedAttribute">
            <xsl:sort select="@Name"/>
          </xsl:apply-templates>
        </table>
      </xsl:if>

      <xsl:if test="AddedElement">
        <h2>Added Element:</h2>
        <table>
          <tr>
            <th>Context</th>
            <th>Name</th>
            <th>Severity</th>
          </tr>
          <xsl:apply-templates select="AddedElement">
            <xsl:sort select="@Name"/>
          </xsl:apply-templates>
        </table>
      </xsl:if>

      <xsl:if test="RemovedElement">
        <h2>Removed Element:</h2>
        <table>
          <tr>
            <th>Context</th>
            <th>Name</th>
            <th>Severity</th>
          </tr>
          <xsl:apply-templates select="RemovedElement">
            <xsl:sort select="@Name"/>
          </xsl:apply-templates>
        </table>
      </xsl:if>

      <xsl:if test="ChangedElement">
        <h2>Changed Element:</h2>
        <xsl:apply-templates select="ChangedElement">
          <xsl:sort select="@Name"/>
        </xsl:apply-templates>
      </xsl:if>
    </div>
  </xsl:template>

  <xsl:template match="ChangedAttribute">
    <tr>
      <td>
        <xsl:value-of select="@Name"/>
      </td>
      <td>
        <xsl:value-of select="@ReferenceValue"/>
      </td>
      <td>
        <xsl:value-of select="@NewValue"/>
      </td>
      <td>
        <xsl:value-of select="@Severity"/>
      </td>
    </tr>
  </xsl:template>

  <xsl:template match="AddedElement | RemovedElement">
    <tr>
      <td>
        <xsl:value-of select="@Context"/>
      </td>
      <td>
        <xsl:value-of select="@Name"/>
      </td>
      <td>
        <xsl:value-of select="@Severity"/>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>
