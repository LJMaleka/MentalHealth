using System;
using System.Text;
using UnityEngine;

public static class PHQ9ReportGenerator
{
    public static string LastGeneratedHtml { get; private set; }

    // Builds the report and keeps it in memory only. Nothing is written to the headset,
    // so the report disappears when the app closes and the next user cannot find it.
    public static string Generate(int[] answers, int totalScore, string language,
                                  string severity, string guidance)
    {
        LastGeneratedHtml = BuildHtml(answers, totalScore, language, severity, guidance);
        return LastGeneratedHtml;
    }

    // Call this when a session ends so the report is gone from memory too.
    public static void Clear() => LastGeneratedHtml = null;

    private static string BuildHtml(int[] answers, int totalScore, string language,
                                    string severity, string guidance)
    {
        var ui = PHQ9Data.GetUIStrings(language);

        string timestamp = DateTime.Now.ToString("dd MMM yyyy, HH:mm");
        string langLabel = language.ToUpper();

        var rows = new StringBuilder();
        for (int i = 0; i < answers.Length; i++)
        {
            int score    = answers[i] >= 0 ? answers[i] : 0;
            string label = score >= 0 && score < 4
                           ? PHQ9Data.GetAnswerLabel(score, language) : "-";
            string question = PHQ9Data.GetQuestionText(i, language);
            rows.Append($@"
      <tr>
        <td style='width:32px;color:#888;'>{i + 1}</td>
        <td>{Escape(question)}</td>
        <td><span class='chip'>{Escape(label)}</span></td>
        <td style='text-align:center;font-weight:bold;'>{score}</td>
      </tr>");
        }

        return $@"<!DOCTYPE html>
<html lang='{Escape(language)}'>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width,initial-scale=1'>
<title>{Escape(ui.reportTitle)}</title>
<style>
  *{{box-sizing:border-box;margin:0;padding:0}}
  body{{font-family:Arial,sans-serif;background:#f0f4f4;padding:20px;color:#333}}
  .card{{background:#fff;border-radius:12px;padding:22px;margin-bottom:16px;
         box-shadow:0 2px 8px rgba(0,0,0,.07)}}
  .header{{background:#1A9E9E;color:#fff;border-radius:12px;padding:22px;margin-bottom:16px}}
  .header h1{{font-size:20px;margin-bottom:6px}}
  .header p{{font-size:13px;opacity:.85}}
  .score-row{{display:flex;align-items:center;gap:20px}}
  .circle{{width:88px;height:88px;border-radius:50%;background:#1A9E9E;color:#fff;
           display:flex;flex-direction:column;align-items:center;justify-content:center;
           flex-shrink:0}}
  .circle .num{{font-size:36px;font-weight:bold;line-height:1}}
  .circle .lbl{{font-size:11px;opacity:.8}}
  .severity{{font-size:20px;font-weight:bold;color:#1A9E9E}}
  .sub{{font-size:13px;color:#888;margin-top:4px}}
  table{{width:100%;border-collapse:collapse;font-size:14px}}
  th{{background:#f0fafa;color:#1A9E9E;text-align:left;padding:10px 12px;
      border-bottom:2px solid #1A9E9E}}
  td{{padding:9px 12px;border-bottom:1px solid #eee;vertical-align:top}}
  tr:last-child td{{border-bottom:none}}
  .chip{{background:#e0f5f5;color:#1A9E9E;border-radius:12px;padding:2px 10px;
         font-size:13px;white-space:nowrap}}
  .guidance{{background:#fff9e6;border-left:4px solid #f0a500;
             border-radius:0 8px 8px 0;padding:16px;white-space:pre-line}}
  .guidance strong{{color:#b07800}}
</style>
</head>
<body>

<div class='header'>
  <h1>{Escape(ui.reportTitle)}</h1>
  <p>{Escape(timestamp)} &nbsp;|&nbsp; {Escape(langLabel)}</p>
</div>

<div class='card'>
  <div class='score-row'>
    <div class='circle'>
      <span class='num'>{totalScore}</span>
      <span class='lbl'>/ 27</span>
    </div>
    <div>
      <div class='severity'>{Escape(severity)}</div>
      <div class='sub'>{Escape(ui.resultsTotalLabel)}</div>
    </div>
  </div>
</div>

<div class='card'>
  <h3 style='margin-bottom:14px;color:#333'>{Escape(ui.reportResponsesHeading)}</h3>
  <table>
    <tr>
      <th>#</th>
      <th>{Escape(ui.reportColQuestion)}</th>
      <th>{Escape(ui.reportColAnswer)}</th>
      <th style='text-align:center'>{Escape(ui.reportColScore)}</th>
    </tr>
    {rows}
  </table>
</div>

<div class='card guidance'>
  <strong>{Escape(ui.reportGuidanceLabel)}:&nbsp;</strong>{Escape(guidance)}
</div>

</body>
</html>";
    }

    private static string Escape(string s) =>
        s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;")
         .Replace("\"", "&quot;").Replace("'", "&#39;");
}
