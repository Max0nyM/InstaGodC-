using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using xNet;

namespace InstaGod_By_MadMax
{
    class InstaGodApi
    {


        public void before(UserInfo User)
        {
       
            SendRequest("news/inbox/?activity_module=all", "", User,"GET");
            var data = "is_prefetch=0&seen_posts=&phone_id="+User.phone_id+"&battery_level=98&timezone_offset=10800&_csrftoken="+User.cookie["csrftoken"] +"&is_pull_to_refresh=0&_uuid="+User.uuid+"&unseen_posts=&is_charging=0&recovered_from_crash=1";
            SendRequest("feed/timeline/", data, User,"POST1");
            SendRequest("feed/reels_tray/", "", User, "GET");
            SendRequest("direct_v2/inbox/", "", User, "GET");
            SendRequest("discover/explore/", "", User, "GET");
       
        }

        

        public void follow(string userid, UserInfo User)
        {
      
           string pg =  SendRequest("friendships/show/" + userid + "/", "", User, "GET").ToString();
           SendRequest("users/" + User.cookie["ds_user_id"] + "/info/", "", User, "GET");
            var data = "";
            if (pg.Contains("\"is_private\": false")) { 
            pg = SendRequest("feed/user/" + userid + "/", "", User, "GET").ToString();
                if (pg.Contains("\"id\": \""))
                {
                    string media = Functions.Pars(pg,"\"id\": \"", "\",", 0);
                    data = "{\"module_name\":\"feed_contextual_post\",\"media_id\":\""+media+"\",\"_csrftoken\":\""+User.cookie["csrftoken"] + "\",\"radio_type\":\"wifi-none\",\"_uid\":\"" + User.cookie["ds_user_id"] +"\",\"_uuid\":\""+User.uuid+"\"}";
                    SendRequest("media/" + media + "/like/", data, User);
                }
            }
           data = "{\"_csrftoken\":\""+ User.cookie["csrftoken"] + "\",\"_uid\":\""+ User.cookie["ds_user_id"] + "\",\"_uuid\":\""+User.uuid+"\",\"user_id\":\""+userid+"\"}";
           SendRequest("friendships/create/"+userid+"/", data, User);
           SendRequest("friendships/show/" + userid + "/", "", User, "GET");
        }

        public int like(string media_id, UserInfo User)
        {
           var data = "{\"_uuid\":\"" + User.phone_id + "\",\"_uid\":\"" + User.login + "\",\"media_id\":\"" + media_id + "\"} ";
            var respoonse = SendRequest("media/" + media_id + "/like/", data, User);
            if (respoonse == null)
                return 0;
            else
                return 1;
        }

        public int unfollow(string userid, UserInfo User)
        {
            if (userid.Contains("_"))
                userid = userid.Split('_')[1];
            var data = "{\"_uuid\":\"" + User.phone_id + "\",\"_uid\":\"" + User.login + "\",\"user_id\":\"" + userid + "\"} ";
            var respoonse = SendRequest("friendships/destroy/"+ userid+"/", data, User);
            if (respoonse == null)
                return 0;
            else
                return 1;
        }

        
        public JObject GetUserFollowowing(string usernameId, string maxid, UserInfo User)
        {
            try
            {

     
            if (usernameId == "")
                usernameId = Functions.Pars(User.cookie.ToString() + ";", "ds_user_id=", ";", 0);
            string strJson = SendRequest("friendships/" + usernameId + "/following/?max_id=" + maxid, "", User).ToString();
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            return rss;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public JObject GetUserFollowers(string usernameId, string maxid, UserInfo User)
        {
            try
            {

                if (usernameId == "")
                usernameId = Functions.Pars(User.cookie.ToString()+";", "ds_user_id=", ";", 0);
            string strJson = SendRequest("friendships/" + usernameId + "/followers/?max_id=" + maxid, "", User).ToString();
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            return rss;
        }
            catch (Exception e)
            {
                return null;
    }
}

        public JObject GetMediaLikers(string media_id, UserInfo User)
        {
            string strJson = SendRequest("media/" + media_id + "/likers/","", User).ToString();
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            return rss;

        }
        public JObject parsUserFeed(string userName, string maxid, UserInfo User)
        {
            string strJson = SendRequest("feed/user/" + userName + "/?max_id=" + maxid, "", User).ToString();
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            return rss;
        }

        public JObject parsTagFeed(string hashtagString,string maxid, UserInfo User)
        {
            string strJson = SendRequest("feed/tag/" + hashtagString + "/?max_id=" + maxid, "", User).ToString();
            var serialized = JsonConvert.DeserializeObject(strJson);
            JObject rss = JObject.Parse(serialized.ToString());
            return rss;
        }

        public JObject GetInfo(UserInfo User)
        {
            try
            {
                HttpRequest http = new HttpRequest();
                http.UserAgent = Http.ChromeUserAgent();
                string strJson = http.Get("https://www.instagram.com/" + User.login + "/?__a=1").ToString();
                var serialized = JsonConvert.DeserializeObject(strJson);
                JObject rss = JObject.Parse(serialized.ToString());
                return rss;
            }
            catch (Exception e)
            {
                return null;
            }
          
        }

        public CookieDictionary login(UserInfo User)
        {
            var data = "{\"id\":\"" + Guid.NewGuid() +
                       "\",\"experiments\":\"ig_android_reg_login_btn_active_state,ig_android_ci_opt_in_at_reg,ig_android_one_click_in_old_flow,ig_android_merge_fb_and_ci_friends_page,ig_android_non_fb_sso,ig_android_mandatory_full_name,ig_android_reg_enable_login_password_btn,ig_android_reg_phone_email_active_state,ig_android_analytics_data_loss,ig_fbns_blocked,ig_android_contact_point_triage,ig_android_reg_next_btn_active_state,ig_android_prefill_phone_number,ig_android_show_fb_social_context_in_nux,ig_android_one_tap_login_upsell,ig_fbns_push,ig_android_phoneid_sync_interval\"}";
            User.cookie = SendRequest("qe/sync/", data, User).Cookies;

            data = "{\"phone_id\":\"" + User.phone_id + "\",\"csrftoken\":\"" + "\",\"device_id\":\"" + User.device_id +
                   "\",\"guid\":\"" + User.guid + "\",\"username\":\"" + User.login + "\",\"password\":\"" +
                   User.password + "\",\"login_attempt_count\":\"0\"}";

            User.cookie = SendRequest("accounts/login/", data, User).Cookies;

            data = "{\"_csrftoken\":\""+User.cookie["csrftoken"]+"\",\"id\":\"" + User.cookie["ds_user_id"] + "\",\"_uid\":\"" + User.cookie["ds_user_id"] + "\",\"_uuid\":\""+User.uuid+"\",\"experiments\":\"ig_android_ad_holdout_16m5_universe,ig_android_progressive_jpeg,ig_creation_growth_holdout,ig_android_oppo_app_badging,ig_android_ad_remove_username_from_caption_universe,ig_android_enable_share_to_whatsapp,ig_android_direct_drawing_in_quick_cam_universe,ig_android_ad_always_send_ad_attribution_id_universe,ig_android_universe_video_production,ig_android_direct_plus_button,ig_android_ads_heatmap_overlay_universe,ig_android_http_stack_experiment_2016,ig_android_infinite_scrolling,ig_fbns_blocked,ig_android_post_auto_retry_v7_21,ig_fbns_push,ig_android_video_playback_bandwidth_threshold,ig_android_direct_link_preview,ig_android_direct_typing_indicator,ig_android_preview_capture,ig_android_feed_pill,ig_android_profile_link_iab,ig_android_story_caption,ig_android_network_cancellation,ig_android_histogram_reporter,ig_android_anrwatchdog,ig_android_search_client_matching,ig_android_follow_request_text_buttons,ig_android_feed_zoom,ig_android_drafts_universe,ig_android_disable_comment,ig_android_user_detail_endpoint,ig_android_os_version_blocking,ig_android_blocked_list,ig_android_event_creation,ig_android_high_res_upload_2,ig_android_2fac,ig_android_mark_reel_seen_on_Swipe_forward,ig_android_comment_redesign,ig_android_ad_sponsored_label_universe,ig_android_mentions_dismiss_rule,ig_android_disable_chroma_subsampling,ig_android_share_spinner,ig_android_video_reuse_surface,ig_explore_v3_android_universe,ig_android_media_favorites,ig_android_nux_holdout,ig_android_insta_video_universe,ig_android_search_null_state,ig_android_universe_reel_video_production,liger_instagram_android_univ,ig_android_direct_emoji_picker,ig_feed_holdout_universe,ig_android_direct_send_auto_retry_universe,ig_android_samsung_app_badging,ig_android_disk_usage,ig_android_business_promotion,ig_android_direct_swipe_to_inbox,ig_android_feed_reshare_button_nux,ig_android_react_native_boost_post,ig_android_boomerang_feed_attribution,ig_fbns_shared,ig_fbns_dump_ids,ig_android_react_native_universe,ig_show_promote_button_in_feed,ig_android_ad_metadata_behavior_universe,ig_android_video_loopcount_int,ig_android_inline_gallery_backoff_hours_universe,ig_android_rendering_controls,ig_android_profile_photo_as_media,ig_android_async_stack_image_cache,ig_video_max_duration_qe_preuniverse,ig_video_copyright_whitelist,ig_android_render_stories_with_content_override,ig_android_ad_intent_to_highlight_universe,ig_android_swipe_navigation_x_angle_universe,ig_android_disable_comment_public_test,ig_android_profile,ig_android_direct_blue_tab,ig_android_enable_share_to_messenger,ig_android_fetch_reel_tray_on_resume_universe,ig_android_promote_again,ig_feed_event_landing_page_channel,ig_ranking_following,ig_android_pending_request_search_bar,ig_android_feed_ufi_redesign,ig_android_pending_edits_dialog_universe,ig_android_business_conversion_flow_universe,ig_android_show_your_story_when_empty_universe,ig_android_ad_drop_cookie_early,ig_android_app_start_config,ig_android_fix_ise_two_phase,ig_android_ppage_toggle_universe,ig_android_pbia_normal_weight_universe,ig_android_profanity_filter,ig_ios_su_activity_feed,ig_android_search,ig_android_boomerang_entry,ig_android_mute_story,ig_android_inline_gallery_universe,ig_android_ad_remove_one_tap_indicator_universe,ig_android_view_count_decouple_likes_universe,ig_android_contact_button_redesign_v2,ig_android_periodic_analytics_upload_v2,ig_android_send_direct_typing_indicator,ig_android_ad_holdout_16h2m1_universe,ig_android_react_native_comment_moderation_settings,ig_video_use_sve_universe,ig_android_inline_gallery_no_backoff_on_launch_universe,ig_android_immersive_viewer,ig_android_discover_people_icon,ig_android_profile_follow_back_button,is_android_feed_seen_state,ig_android_dense_feed_unit_cards,ig_android_drafts_video_universe,ig_android_exoplayer,ig_android_add_to_last_post,ig_android_ad_remove_cta_chevron_universe,ig_android_ad_comment_cta_universe,ig_android_search_event_icon,ig_android_channels_home,ig_android_feed,ig_android_dv2_realtime_private_share,ig_android_non_square_first,ig_android_video_interleaved_v2,ig_android_video_cache_policy,ig_android_react_native_universe_kill_switch,ig_android_video_captions_universe,ig_android_follow_search_bar,ig_android_last_edits,ig_android_two_step_capture_flow,ig_android_video_download_logging,ig_android_share_link_to_whatsapp,ig_android_facebook_twitter_profile_photos,ig_android_swipeable_filters_blacklist,ig_android_ad_pbia_profile_tap_universe,ig_android_use_software_layer_for_kc_drawing_universe,ig_android_react_native_ota,ig_android_direct_mutually_exclusive_experiment_universe,ig_android_following_follower_social_context\"}";
            User.cookie = SendRequest("qe/sync/", data, User).Cookies;
         
            return SendRequest("users/" + User.cookie["ds_user_id"] + "/info/", data, User,"GET").Cookies;
     

        }

        public HttpResponse SendRequest(string path,string data,UserInfo User,string method="POST")
        {
            try
            {

         
            HttpRequest http = new HttpRequest();
            CookieDictionary Cookie = new CookieDictionary();
            var reqParams = new RequestParams();
            http.UserAgent = User.agent;
                if (User.cookie != null)
                {
                    http.Cookies = User.cookie;
                }
                else if (User.cookie != null && User.cookie.ToString().Contains("session"))
                {
                    http.Cookies = User.cookie;
                }
                else
                {
                    http.Cookies = Cookie;
                }
                http.KeepAlive = false;
            http.AddHeader(HttpHeader.Accept, "*/*");
            http.AddHeader(HttpHeader.AcceptLanguage, "en-US");
            http.AddHeader("X-IG-Connection-Type", "WIFI");
            http.AddHeader("X-IG-Capabilities", "36o=");
            http.AllowAutoRedirect = true;
            http.ConnectTimeout = 20000;
            http.ReadWriteTimeout = 20000;
            http.EnableEncodingContent = true;
                if (method == "POST") { 
            char[] charsToTrim = { ' ' };
            reqParams.Clear();
           
            var sig = Functions.GenerateSignature(data);
            sig.Trim(charsToTrim);
            reqParams["signed_body"] = sig.ToLower() + "." + data;
            reqParams["ig_sig_key_version"] = "4";
            http.Post("https://i.instagram.com/api/v1/" + path, reqParams);
                }
                else
                   if (method == "GET")
                {
                http.Get("https://i.instagram.com/api/v1/" + path);
           }
                   else
                   {

                    var reqS = data.Split('&');
                       foreach (var req in reqS)
                       {

                        reqParams[req.Split('=')[0]] = req.Split('=')[1];
                    }
                   
                    http.Post("https://i.instagram.com/api/v1/" + path, reqParams);
                }
                return http.Response;
            }
            catch (Exception e)
            {
                return null;
            }
          
        }
    }
}
