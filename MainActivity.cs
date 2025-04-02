using Android.Text.Style;
using Android.Text;
using System.Text.RegularExpressions;
using Android.Graphics;

namespace LuaScriptEditor;

[Activity(Label = "@string/app_name", MainLauncher = true)]
public sealed partial class MainActivity : Activity
{
    private Button? btnRunScript, btnGoBack, btnClear;
    private EditText? etLuaScript;
    private TextView? tvOutput;
    private string storedLuaCode = string.Empty;

    internal static readonly System.Text.StringBuilder scriptOutput = new();

    internal string EvaluateLuaScriptOutput()
    {
        try
        {
            using NLua.Lua lua = new();
            // Redirect print to our StringBuilder
            lua.RegisterFunction("print", typeof(MainActivity).GetMethod("LuaPrint"));

            // Execute the script
            var results = lua.DoString(storedLuaCode);

            // Add any return values to output
            if (results is not null && results.Length > 0)
            {
                scriptOutput.AppendLine("\n-- Return value(s) --");
                foreach (var item in results)
                    scriptOutput.AppendLine(item?.ToString());
            }

            return scriptOutput.ToString();
        }
        catch (Exception ex)
        {
            return $"Error executing Lua script:\n{ex.Message}";
        }
    }

    public static void LuaPrint(params object[] objects)
    {
        if (objects.Length == 0)
            return;
        foreach (var obj in objects)
            scriptOutput.Append((obj ?? "nil").ToString() + ' ');

        scriptOutput.AppendLine();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        ShowMainPage(null, EventArgs.Empty);
    }
    public void ShowMainPage(object? sender, EventArgs e)
    {
        SetContentView(Resource.Layout.activity_main);
        scriptOutput.Clear();

        btnRunScript = FindViewById<Button>(Resource.Id.btnRunScript);
        btnClear = FindViewById<Button>(Resource.Id.btnClear);
        etLuaScript = FindViewById<EditText>(Resource.Id.etLuaScript);
        ArgumentNullException.ThrowIfNull(etLuaScript);

        ApplySyntaxHighlighting(null, EventArgs.Empty);
        etLuaScript.Text = storedLuaCode;
        etLuaScript.TextChanged += ApplySyntaxHighlighting;

        if (btnRunScript is not null)
            btnRunScript.Click += ShowOutputResult;

        if (btnClear is not null)
            btnClear.Click += (sender, e) => etLuaScript.Text = string.Empty;
    }

    public void ShowOutputResult(object? sender, EventArgs e)
    {
        SetContentView(Resource.Layout.output_result);

        btnGoBack = FindViewById<Button>(Resource.Id.btnGoBack);
        tvOutput = FindViewById<TextView>(Resource.Id.tvOutput);

        if (btnGoBack is not null)
            btnGoBack.Click += ShowMainPage;

        // Get the Lua code from the editor
        storedLuaCode = etLuaScript?.Text ?? string.Empty;

        // Evaluate the script and show results
        ArgumentNullException.ThrowIfNull(tvOutput);
        tvOutput.Text = EvaluateLuaScriptOutput();
    }

    [GeneratedRegex(@"\b\w+\b", RegexOptions.Compiled)]
    private static partial Regex IdentifierRegex();

    [GeneratedRegex(@"\b(local|nil|true|false|and|or|not)\b", RegexOptions.Compiled)]
    private static partial Regex KeywordRegex();

    [GeneratedRegex(@"\b(assert|collectgarbage|dofile|error|(get|set)metatable|ipairs|load(file)?|next|pairs|x?pcall|print|raw(equal|get|len|set)?|select|to(number|string)|type|unpack)\b", RegexOptions.Compiled)]
    private static partial Regex BuiltinsRegex();

    [GeneratedRegex(@"\b(break|do|else|elseif|end|for|function|goto|if|in|repeat|return|then|until|while)\b", RegexOptions.Compiled)]
    private static partial Regex SnippetRegex();

    [GeneratedRegex(@"(""[^""\\]*(?:\\.[^""\\]*)*""|'[^'\\]*(?:\\.[^'\\]*)*')", RegexOptions.Compiled)]
    private static partial Regex StringRegex();

    [GeneratedRegex(@"(--[^\n]*|\[\[.*?\]\])", RegexOptions.Singleline)]
    private static partial Regex CommentRegex();

    [GeneratedRegex(@"\b\d+(\.\d+)?\b", RegexOptions.Compiled)]
    private static partial Regex NumberRegex();

    private bool isHighlighting = false;
    private void ApplySyntaxHighlighting(object? sender, EventArgs e)
    {
        if (isHighlighting || etLuaScript is null) return;

        etLuaScript.SetTextColor(Color.Rgb(212, 212, 212));
        isHighlighting = true;
        var codeText = etLuaScript.Text;
        List<int> strPosList = [];

        try
        {
            if (string.IsNullOrEmpty(codeText)) return;

            // Store cursor position
            int selectionStart = etLuaScript.SelectionStart;
            int selectionEnd = etLuaScript.SelectionEnd;

            var spannable = new SpannableString(codeText);

            // Hightlight identifiers
            foreach (Match match in IdentifierRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(156, 220, 254)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            // Highlight numbers
            foreach (Match match in NumberRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(181, 206, 168)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            // Highlight general keywords
            foreach (Match match in KeywordRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(83, 151, 207)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            // Highlight snippet keywords
            foreach (Match match in SnippetRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(197, 132, 192)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            // Hightlight builtin functions (without prefixes)
            foreach (Match match in BuiltinsRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(220, 220, 170)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            // Highlight strings
            foreach (Match match in StringRegex().Matches(codeText))
            {
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(206, 145, 120)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
                strPosList.AddRange(Enumerable.Range(match.Index, match.Length));
            }

            // Highlight comments
            foreach (Match match in CommentRegex().Matches(codeText))
            {
                if (strPosList.Contains(match.Index))
                    continue;
                spannable.SetSpan(new ForegroundColorSpan(Color.Rgb(106, 153, 85)),
                                  match.Index, match.Index + match.Length,
                                  SpanTypes.ExclusiveExclusive);
            }

            etLuaScript.TextFormatted = spannable;
            etLuaScript.SetSelection(selectionStart, selectionEnd);
        }
        finally
        {
            isHighlighting = false;
        }
    }
}