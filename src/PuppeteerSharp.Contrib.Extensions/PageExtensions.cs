﻿using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Contrib.Extensions
{
    /// <summary>
    /// <see cref="Page"/> extension methods.
    /// </summary>
    public static class PageExtensions
    {
        // Query

        /// <summary>
        /// The method runs <c>document.querySelectorAll</c> within the page and then tests a <c>RegExp</c> against the elements <c>textContent</c>. The first element match is returned. If no element matches the selector and regular expression, the return value resolve to <c>null</c>.
        /// See also https://stackoverflow.com/a/37098508
        /// </summary>
        /// <param name="page">A <see cref="Page"/> to query</param>
        /// <param name="selector">A selector to query page for</param>
        /// <param name="regex">A regular expression to test against <c>element.textContent</c></param>
        /// <returns>Task which resolves to <see cref="ElementHandle"/> pointing to the frame element</returns>
        public static async Task<ElementHandle> QuerySelectorWithContentAsync(this Page page, string selector, string regex)
        {
            var handle = await page.EvaluateFunctionHandleAsync(
                @"(selector, regex) => {
                    var elements = document.querySelectorAll(selector);
                    return Array.prototype.find.call(elements, function(element) {
                        return RegExp(regex).test(element.textContent);
                    });
                }", selector, regex).ConfigureAwait(false) as ElementHandle;

            return handle;
        }

        /// <summary>
        /// The method runs <c>document.querySelectorAll</c> within the page and then tests a <c>RegExp</c> against the elements <c>textContent</c>. All element matches are returned. If no element matches the selector and regular expression, the return value resolve to <see cref="System.Array.Empty{T}"/>.
        /// See also https://stackoverflow.com/a/37098508
        /// </summary>
        /// <param name="page">A <see cref="Page"/> to query</param>
        /// <param name="selector">A selector to query page for</param>
        /// <param name="regex">A regular expression to test against <c>element.textContent</c></param>
        /// <returns>Task which resolves to ElementHandles pointing to the frame elements</returns>
        public static async Task<ElementHandle[]> QuerySelectorAllWithContentAsync(this Page page, string selector, string regex)
        {
            var arrayHandle = await page.EvaluateFunctionHandleAsync(
                @"(selector, regex) => {
                    var elements = document.querySelectorAll(selector);
                    return Array.prototype.filter.call(elements, function(element) {
                        return RegExp(regex).test(element.textContent);
                    });
                }", selector, regex).ConfigureAwait(false);

            var properties = await arrayHandle.GetPropertiesAsync().ConfigureAwait(false);
            await arrayHandle.DisposeAsync().ConfigureAwait(false);

            return properties.Values.OfType<ElementHandle>().ToArray();
        }
    }
}