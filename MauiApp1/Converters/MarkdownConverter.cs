using System.Globalization;

namespace MauiApp1.Converters
{
    public class MarkdownConverter : IValueConverter
    {
        // 获取当前主题下的主要文字颜色 - 固定为白色
        private Color GetThemeTextColor()
        {
            // AI消息文字固定为白色，不受主题影响
            return Colors.White;
        }
        
        // 获取当前主题下的引用文字颜色 - 固定为浅灰色
        private Color GetThemeQuoteColor()
        {
            // AI消息引用文字固定为浅灰色，不受主题影响
            return Colors.LightGray;
        }
        
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string text || string.IsNullOrEmpty(text))
                return new FormattedString();

            var formattedString = new FormattedString();
            var lines = text.Split('\n');
            var inCodeBlock = false;
            var codeBlockLanguage = string.Empty;
            var codeBlockContent = new List<string>();
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // 添加空行
                    formattedString.Spans.Add(new Span { Text = "\n" });
                    continue;
                }

                // 处理代码块开始 (```)
                if (line.StartsWith("```"))
                {
                    if (!inCodeBlock)
                    {
                        // 开始代码块
                        inCodeBlock = true;
                        codeBlockLanguage = line.Substring(3).Trim();
                        codeBlockContent.Clear();
                        
                        // 添加语言标签
                        if (!string.IsNullOrEmpty(codeBlockLanguage))
                        {
                            formattedString.Spans.Add(new Span
                            {
                                Text = $"[{codeBlockLanguage}]\n",
                                FontSize = 12,
                                FontAttributes = FontAttributes.Italic,
                                TextColor = Colors.Gray
                            });
                        }
                    }
                    else
                    {
                        // 结束代码块
                        inCodeBlock = false;
                        
                        // 添加代码块内容
                        var codeText = string.Join("\n", codeBlockContent);
                        formattedString.Spans.Add(new Span
                        {
                            Text = codeText + "\n",
                            FontFamily = "Consolas",
                            BackgroundColor = Color.FromRgb(30, 30, 30),
                            TextColor = Color.FromRgb(220, 220, 220),
                            FontSize = 13
                        });
                        
                        codeBlockContent.Clear();
                    }
                    continue;
                }

                // 如果在代码块内，收集内容
                if (inCodeBlock)
                {
                    codeBlockContent.Add(line);
                    continue;
                }

                // 处理标题 (# ## ###)
                if (line.StartsWith("#"))
                {
                    var headerLevel = line.TakeWhile(c => c == '#').Count();
                    var headerText = line.Substring(headerLevel).Trim();
                    
                    if (!string.IsNullOrEmpty(headerText))
                    {
                        var fontSize = headerLevel switch
                        {
                            1 => 24,
                            2 => 20,
                            3 => 18,
                            4 => 16,
                            _ => 14
                        };
                        
                        formattedString.Spans.Add(new Span
                        {
                            Text = headerText + "\n",
                            FontSize = fontSize,
                            FontAttributes = FontAttributes.Bold,
                            TextColor = GetThemeTextColor()
                        });
                        continue;
                    }
                }

                // 处理列表项 (- * +)
                if (line.TrimStart().StartsWith("- ") || line.TrimStart().StartsWith("* ") || line.TrimStart().StartsWith("+ "))
                {
                    var listText = line.TrimStart().Substring(2);
                                            formattedString.Spans.Add(new Span
                        {
                            Text = "• " + listText + "\n",
                            TextColor = GetThemeTextColor()
                        });
                    continue;
                }

                // 处理数字列表 (1. 2. 3.)
                if (System.Text.RegularExpressions.Regex.IsMatch(line.TrimStart(), @"^\d+\.\s"))
                {
                    var listText = line.TrimStart();
                                            formattedString.Spans.Add(new Span
                        {
                            Text = listText + "\n",
                            TextColor = GetThemeTextColor()
                        });
                    continue;
                }

                // 处理引用 (>)
                if (line.StartsWith(">"))
                {
                    var quoteText = line.Substring(1).Trim();
                                            formattedString.Spans.Add(new Span
                        {
                            Text = quoteText + "\n",
                            TextColor = GetThemeQuoteColor(),
                            FontAttributes = FontAttributes.Italic
                        });
                    continue;
                }

                // 处理普通文本（包含行内代码、粗体、斜体）
                var parts = ParseInlineCode(line);
                foreach (var part in parts)
                {
                    if (part.IsCode)
                    {
                        formattedString.Spans.Add(new Span
                        {
                            Text = part.Text,
                            FontFamily = "Consolas",
                            BackgroundColor = Color.FromRgb(40, 40, 40),
                            TextColor = Color.FromRgb(220, 220, 220),
                            FontSize = 13
                        });
                    }
                    else
                    {
                        // 处理粗体和斜体
                        var formattedParts = ParseBoldItalic(part.Text);
                        foreach (var formattedPart in formattedParts)
                        {
                            formattedString.Spans.Add(new Span
                            {
                                Text = formattedPart.Text,
                                FontAttributes = formattedPart.FontAttributes,
                                TextColor = GetThemeTextColor()
                            });
                        }
                    }
                }
                
                formattedString.Spans.Add(new Span { Text = "\n" });
            }

            return formattedString;
        }

        private List<TextPart> ParseInlineCode(string text)
        {
            var parts = new List<TextPart>();
            var currentIndex = 0;
            
            while (currentIndex < text.Length)
            {
                var codeStart = text.IndexOf('`', currentIndex);
                if (codeStart == -1)
                {
                    // 没有更多代码块，添加剩余文本
                    parts.Add(new TextPart
                    {
                        Text = text.Substring(currentIndex),
                        IsCode = false
                    });
                    break;
                }
                
                // 添加代码块前的文本
                if (codeStart > currentIndex)
                {
                    parts.Add(new TextPart
                    {
                        Text = text.Substring(currentIndex, codeStart - currentIndex),
                        IsCode = false
                    });
                }
                
                // 查找代码块结束
                var codeEnd = text.IndexOf('`', codeStart + 1);
                if (codeEnd == -1)
                {
                    // 没有结束标记，当作普通文本
                    parts.Add(new TextPart
                    {
                        Text = text.Substring(currentIndex),
                        IsCode = false
                    });
                    break;
                }
                
                // 添加代码块
                var codeText = text.Substring(codeStart + 1, codeEnd - codeStart - 1);
                parts.Add(new TextPart
                {
                    Text = codeText,
                    IsCode = true
                });
                
                currentIndex = codeEnd + 1;
            }
            
            return parts;
        }

        private List<FormattedPart> ParseBoldItalic(string text)
        {
            var parts = new List<FormattedPart>();
            var currentIndex = 0;
            
            while (currentIndex < text.Length)
            {
                // 查找粗体标记 (**)
                var boldStart = text.IndexOf("**", currentIndex);
                if (boldStart != -1)
                {
                    var boldEnd = text.IndexOf("**", boldStart + 2);
                    if (boldEnd != -1)
                    {
                        // 添加粗体前的文本
                        if (boldStart > currentIndex)
                        {
                            parts.Add(new FormattedPart
                            {
                                Text = text.Substring(currentIndex, boldStart - currentIndex),
                                FontAttributes = FontAttributes.None
                            });
                        }
                        
                        // 添加粗体文本
                        var boldText = text.Substring(boldStart + 2, boldEnd - boldStart - 2);
                        parts.Add(new FormattedPart
                        {
                            Text = boldText,
                            FontAttributes = FontAttributes.Bold
                        });
                        
                        currentIndex = boldEnd + 2;
                        continue;
                    }
                }
                
                // 查找斜体标记 (*)
                var italicStart = text.IndexOf("*", currentIndex);
                if (italicStart != -1)
                {
                    var italicEnd = text.IndexOf("*", italicStart + 1);
                    if (italicEnd != -1 && italicEnd > italicStart + 1)
                    {
                        // 添加斜体前的文本
                        if (italicStart > currentIndex)
                        {
                            parts.Add(new FormattedPart
                            {
                                Text = text.Substring(currentIndex, italicStart - currentIndex),
                                FontAttributes = FontAttributes.None
                            });
                        }
                        
                        // 添加斜体文本
                        var italicText = text.Substring(italicStart + 1, italicEnd - italicStart - 1);
                        parts.Add(new FormattedPart
                        {
                            Text = italicText,
                            FontAttributes = FontAttributes.Italic
                        });
                        
                        currentIndex = italicEnd + 1;
                        continue;
                    }
                }
                
                // 没有找到标记，添加剩余文本
                parts.Add(new FormattedPart
                {
                    Text = text.Substring(currentIndex),
                    FontAttributes = FontAttributes.None
                });
                break;
            }
            
            return parts;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TextPart
    {
        public string Text { get; set; } = string.Empty;
        public bool IsCode { get; set; }
    }

    public class FormattedPart
    {
        public string Text { get; set; } = string.Empty;
        public FontAttributes FontAttributes { get; set; }
    }
}
