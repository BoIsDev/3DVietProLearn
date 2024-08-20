﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.CoreLibrary.NativePlugins;
using VoxelBusters.EssentialKit.CloudServicesCore;

namespace VoxelBusters.EssentialKit
{
    /// <summary>
    /// Provides a cross-platform interface to sync information across various devices by storing it in the cloud.
    /// </summary>
    /// <description>
    /// <para>
    /// You can use to make preference, configuration, and app-state data available to every instance of your app on every device connected to a user’s cloud service account. 
    /// You can store primitive values as well as object types: <c>IList</c> and <c>IDictionary</c>.
    /// </para>
    /// <para>
    /// When you use this feature on iOS device, iCloud service will be used. Whereas on Android, it will use Google Cloud service.
    /// For setup instructions on iOS, read <a href="https://developer.apple.com/library/ios/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html#//apple_ref/doc/uid/TP40012582-CH26-SW2">Adding iCloud Support</a>, 
    /// Similarly for Android, see <a href=""></a>.
    /// </para>
    /// </description>
    /// <remarks>
    /// \note
    /// <para>
    /// On iOS, the total amount of space available to store key-value data, for a given user, is 1 MB. There is a per-key value size limit of 1 MB, and a maximum of 1024 keys.
    /// If you attempt to write data that exceeds these quotas, the write attempt fails and no change is made to your cloud.
    /// In this scenario, the system posts the <see cref="OnSavedDataChange"/> with a change reason of <see cref="eCloudDataStoreValueChangeReason.QUOTA_VIOLATION"/>.
    /// </para>
    /// </remarks>
    public static class CloudServices
    {
        #region Constants

        private     const   string                          kCacheKeyUserId     = "$userId";

        #endregion

        #region Static fields

        [ClearOnReload]
        private     static  INativeCloudServicesInterface   s_nativeInterface;
        [ClearOnReload]
        private     static  KeyValueDataStore               s_localCache;

        #endregion

        #region Static properties

         private     static  INativeCloudServicesInterface    NativeInterface
        {
            get
            {
                if(s_nativeInterface == null)
                {
                    DebugLogger.LogException(EssentialKitDomain.Default, new Exception("Cloud Services is not enabled. You need to enable this feature to use it."));
                }

                return s_nativeInterface;
            }

            set
            {
                s_nativeInterface = value;
            }
        }

        public static CloudServicesUnitySettings UnitySettings { get; private set; }

        public static CloudUser ActiveUser { get; private set; }

        internal static KeyValueDataStore LocalCache => ObjectHelper.CreateInstanceIfNull(
            reference: ref s_localCache,
            createFunc: () => new KeyValueDataStore(savePath: $"{EssentialKitSettings.PersistentDataPath}/CloudLocalCache.json"));

        #endregion

        #region Static events

        /// <summary>
        /// Event that will be called when cloud user changed.
        /// </summary>
        public static event EventCallback<CloudServicesUserChangeResult> OnUserChange;

        /// <summary>
        /// Event that will be called when the value of one or more keys in the local key-value store changed due to incoming data pushed from cloud.
        /// </summary>
        public static event Callback<CloudServicesSavedDataChangeResult> OnSavedDataChange;

        /// <summary>
        /// Event that will be called when the synchronize request is finished.
        /// </summary>
        public static event Callback<CloudServicesSynchronizeResult> OnSynchronizeComplete;

        #endregion

        #region Setup methods

        public static bool IsAvailable()
        {
            return (NativeInterface != null) && NativeInterface.IsAvailable;
        }

        public static void Initialize(CloudServicesUnitySettings settings)
        {
            Assert.IsArgNotNull(settings, nameof(settings));

            // Reset event properties
            OnUserChange            = null;
            OnSavedDataChange       = null;
            OnSynchronizeComplete   = null;

            // Set default properties
            UnitySettings           = settings;
            ActiveUser              = new CloudUser(userId: null, accountStatus: CloudUserAccountStatus.CouldNotDetermine);

            // Configure interface
            NativeInterface       = NativeFeatureActivator.CreateInterface<INativeCloudServicesInterface>(ImplementationSchema.CloudServices, true);

            RegisterForEvents();
            if (settings.IsEnabled && settings.SynchronizeOnLoad)
            {
                Synchronize();
            }
        }

        #endregion

        #region Get values methods

        /// <summary>
        /// Returns the boolean value associated with the specified key.
        /// </summary>
        /// <returns>The boolean value associated with the specified key, that value is returned. or <c>false</c> if the key was not found.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static bool GetBool(string key)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return default(bool);
            }

            try
            {
                // make request
                return NativeInterface.GetBool(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
                return default(bool);
            }
        }

        /// <summary>
        /// Returns the integer value associated with the specified key.
        /// </summary>
        /// <returns>The integer value associated with the specified key, that value is returned. or <c>0</c> if the key was not found.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static int GetInt(string key)
        {
            return (int)GetLong(key);
        }

        /// <summary>
        /// Returns the long value associated with the specified key.
        /// </summary>
        /// <returns>The long value associated with the specified key or <c>0</c> if the key was not found.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static long GetLong(string key)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return default(long);
            }

            try
            {
                // make request
                return NativeInterface.GetLong(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
                return default(long);
            }
        }

        /// <summary>
        /// Returns the float value associated with the specified key.
        /// </summary>
        /// <returns>The float value associated with the specified key or <c>0</c> if the key was not found.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static float GetFloat(string key)
        {
            return (float)GetDouble(key);
        }

        /// <summary>
        /// Returns the double value associated with the specified key.
        /// </summary>
        /// <returns>The double value associated with the specified key or <c>0</c> if the key was not found.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static double GetDouble(string key)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return default(double);
            }

            try
            {
                // make request
                return NativeInterface.GetDouble(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
                return default(double);
            }
        }

        /// <summary>
        /// Returns the string value associated with the specified key.
        /// </summary>
        /// <returns>The string associated with the specified key, or <c>null</c> if the key was not found or its value is not an <c>string</c> object.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static string GetString(string key)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return null;
            }

            try
            {
                // make request
                return NativeInterface.GetString(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
                return null;
            }
        }

        /// <summary>
        /// Returns the array object associated with the specified key.      
        /// </summary>
        /// <returns>Array object associated with the specified key, or <c>null</c> if the key was not found or its value is not an <c>Array</c> object.</returns>
        /// <param name="key">A string used to identify the value stored in the cloud data store.</param>
        public static byte[] GetByteArray(string key)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return null;
            }

            try
            {
                // make request
                return NativeInterface.GetByteArray(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
                return null;
            }
        }

        #endregion

        #region Set values methods

        /// <summary>
        /// Sets a boolean value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The boolean value to store.</param>
        public static void SetBool(string key, bool value)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return;
            }

            try
            {
                // copy value to the local cache
                LocalCache.SetBool(key, value);

                // make request
                NativeInterface.SetBool(key, value);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        /// <summary>
        /// Sets a interger value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The integer value to store.</param>
        public static void SetInt(string key, int value)
        {
            SetLong(key, value);
        }

        /// <summary>
        /// Sets a long value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The long value to store.</param>
        public static void SetLong(string key, long value)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return;
            }

            try
            {
                // copy value to the local cache
                LocalCache.SetLong(key, value);

                // make request
                NativeInterface.SetLong(key, value);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        /// <summary>
        /// Sets a float value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The float value to store.</param>
        public static void SetFloat(string key, float value)
        {
            SetDouble(key, value);
        }

        /// <summary>
        /// Sets a double value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The double value to store.</param>
        public static void SetDouble(string key, double value)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return;
            }

            try
            {
                // copy value to the local cache
                LocalCache.SetDouble(key, value);

                // make request
                NativeInterface.SetDouble(key, value);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        /// <summary>
        /// Sets a string value for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">The string value to store.</param>
        public static void SetString(string key, string value)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return;
            }

            try
            {
                // copy value to the local cache
                LocalCache.SetString(key, value);

                // make request
                NativeInterface.SetString(key, value);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        /// <summary>
        /// Sets an array object for the specified key in the cloud data store.
        /// </summary>
        /// <param name="key">The key under which to store the value. The length of this key must not exceed 64 bytes.</param>
        /// <param name="value">Array object whose contents has to be stored. The objects in the list must be <c>primitive</c>, <c>IList</c>, <c>IDictionary</c>.</param>
        public static void SetByteArray(string key, byte[] value)
        {
            // validate arguments
            if (string.IsNullOrEmpty(key))
            {
                DebugLogger.LogError(EssentialKitDomain.Default, "Key is null/empty.");
                return;
            }

            try
            {
                // copy value to the local cache
                LocalCache.SetByteArray(key, value);

                // make request
                NativeInterface.SetByteArray(key, value);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        public static IDictionary GetSnapshot()
        {
            return NativeInterface.GetSnapshot();
        }

        #endregion

        #region Sync methods

        /// <summary>
        /// Explicitly synchronizes in-memory data with those stored on disk.
        /// </summary>
        /// <remarks>
        /// \note <see cref="OnSynchronizeComplete"/> is triggered, when your app has completed processing synchronisation request. 
        /// </remarks>
        public static void Synchronize(Callback<CloudServicesSynchronizeResult> callback = null)
        {
            try
            {
                // serialize local copy
                LocalCache.Synchronize();

                // make request
                NativeInterface.Synchronize((success, error) =>
                {
                    // send result to caller object and global listeners
                    var     result  = new CloudServicesSynchronizeResult(success);
                    CallbackDispatcher.InvokeOnMainThread(callback, result);
                    CallbackDispatcher.InvokeOnMainThread(OnSynchronizeComplete, result);
                });
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        #endregion

        #region Remove value methods

        /// <summary>
        /// Removes the value associated with the specified key from the cloud data store.
        /// </summary>
        /// <param name="key">The key corresponding to the value you want to remove.</param>
        public static void RemoveKey(string key)
        { 
            // validate arguments
            Assert.IsNotNullOrEmpty(key, "key");

            try
            {
                // remove value from the local cache
                LocalCache.RemoveKey(key);

                // make request
                NativeInterface.RemoveKey(key);
            }
            catch (Exception exception)
            {
                DebugLogger.LogException(EssentialKitDomain.Default, exception);
            }
        }

        #endregion

        #region Private methods

        private static void RegisterForEvents()
        {
            NativeInterface.OnUserChange          += HandleOnUserChange;
            NativeInterface.OnSavedDataChange     += HandleOnSavedDataChange;
        }

        private static void UnregisterFromEvents()
        {
            NativeInterface.OnUserChange          -= HandleOnUserChange;
            NativeInterface.OnSavedDataChange     -= HandleOnSavedDataChange;
        }

        private static void UpdateLocalCacheOnUserChange(CloudUser user)
        {
            switch (user.AccountStatus)
            {
                case CloudUserAccountStatus.CouldNotDetermine:
                case CloudUserAccountStatus.Restricted:
                    // no action
                    break;

                case CloudUserAccountStatus.NoAccount:
                    // reset cache
                    LocalCache.Clear();
                    break;

                case CloudUserAccountStatus.Available:
                    string  cacheOwnerId    = LocalCache.GetString(key: kCacheKeyUserId);
                    if ((cacheOwnerId != null) && (user.UserId != cacheOwnerId))
                    {
                        LocalCache.Clear();
                        LocalCache.SetString(key: kCacheKeyUserId, user.UserId);
                    }
                    break;
            }
        }

        #endregion

        #region Event callback methods

        private static void HandleOnUserChange(CloudUser user, Error error)
        {
            // update user information
            ActiveUser          = user;

            // check whether cache is dirty
            if (user != null)
            {
                UpdateLocalCacheOnUserChange(user);
            }

            // notify listeners
            var     result      = new CloudServicesUserChangeResult(user: user);
            CallbackDispatcher.InvokeOnMainThread(OnUserChange, result, error);
        }

        private static void HandleOnSavedDataChange(CloudSavedDataChangeReasonCode changeReason, string[] changedKeys)
        {
            // notify listeners
            var     result      = new CloudServicesSavedDataChangeResult(
                changeReason: changeReason,
                changedKeys: changedKeys);
            CallbackDispatcher.InvokeOnMainThread(OnSavedDataChange, result);
        }

        #endregion
    }
}