﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RemoteFork.Network;

namespace RemoteFork.Requestes {
    internal class ParseLinkRequest : BaseRequest {
        public ParseLinkRequest(string text) : base(text) {
        }

        public override async Task<string> Execute() {
            string result = string.Empty;

            string[] array = text.Substring(12).Split('|');
            Console.WriteLine("parse0 " + array[0]);

            string response;
            if (array[0].StartsWith("curl")) {
                var request = new ParseCurlRequest(array[0]);
                response = await request.Execute();
            } else {
                response = await HttpUtility.GetRequest(array[0]);
            }

            if (array.Length == 1) {
                result = response;
            } else {
                if (!array[1].Contains(".*?")) {
                    if (string.IsNullOrEmpty(array[1]) && string.IsNullOrEmpty(array[2])) {
                        result = response;
                    } else {
                        int num1 = response.IndexOf(array[1], StringComparison.Ordinal);
                        if (num1 == -1) {
                            result = string.Empty;
                        } else {
                            num1 += array[1].Length;
                            int num2 = response.IndexOf(array[2], num1, StringComparison.Ordinal);
                            result = num2 == -1
                                ? string.Empty
                                : response.Substring(num1, num2 - num1);
                        }
                    }
                } else {
                    Console.WriteLine(array[1] + "(.*?)" + array[2]);
                    string pattern = array[1] + "(.*?)" + array[2];
                    var regex = new Regex(pattern, RegexOptions.Multiline);
                    var match = regex.Match(response);
                    if (match.Success) {
                        result = match.Groups[1].Captures[0].ToString();
                    }
                }
            }

            return result;
        }
    }
}
