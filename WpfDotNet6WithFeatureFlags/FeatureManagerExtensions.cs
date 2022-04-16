using Microsoft.FeatureManagement;
using System;
using System.Threading.Tasks;

namespace WpfDotNet6WithFeatureFlags
{
    public static class FeatureManagerExtensions
    {
        /// <summary>
        /// Wrapper for IFeatureManager.IsEnabledAsync to use enums instead of strings.
        /// </summary>
        /// <param name="featureManager">The feature manager.</param>
        /// <param name="feature">The feature which must be an enum.</param>
        /// <returns></returns>
        public static async Task<bool> IsEnabledAsync(
            this IFeatureManager featureManager,
            object feature) =>
                await featureManager.IsEnabledAsync(GetFeatureName(feature));

        /// <summary>
        /// Wrapper for IFeatureManager.IsEnabledAsync to use enums instead of strings.
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="featureManager"></param>
        /// <param name="feature"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static async Task<bool> IsEnabledAsync<TContext>(
            this IFeatureManager featureManager,
            object feature,
            TContext context) =>
                await featureManager.IsEnabledAsync(GetFeatureName(feature), context);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="featureManager"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static string? GetFeatureName(object feature)
        {
            var featureType = feature.GetType();
            return featureType.IsEnum 
                ? Enum.GetName(featureType, feature) 
                : throw new ArgumentException($"{nameof(feature)} must be an enum.");
        }

    }
}
