/* *********************************************************************
 * Date: 10 May 2008
 * Created by: Zoltan Juhasz
 * E-Mail: forge@jzo.hu
***********************************************************************/

using System.Collections.Generic;
using System.Linq;
using Forge.Net.TerraGraf.Configuration;

namespace Forge.Net.TerraGraf.Contexts
{

    /// <summary>
    /// Check rules for Network context
    /// </summary>
    internal class NetworkContextRuleManager
    {

        #region Field(s)

        private bool mSeparation = false;

        private Dictionary<string, HashSet<string>> mRules = new Dictionary<string, HashSet<string>>();

        private HashSet<string> mJokers = new HashSet<string>();

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkContextRuleManager"/> class.
        /// </summary>
        internal NetworkContextRuleManager()
        {
            mSeparation = NetworkManager.Instance.InternalConfiguration.NetworkContext.Separation;
            List<ContextRule> contextRules = null;
            if (mSeparation)
            {
                // van szeparáció, whitelist olvasása (kivételek)
                contextRules = NetworkManager.Instance.InternalConfiguration.NetworkContext.WhiteList;
            }
            else
            {
                // blacklist olvasása (tiltások)
                contextRules = NetworkManager.Instance.InternalConfiguration.NetworkContext.BlackList;
            }
            foreach (ContextRule rule in contextRules)
            {
                string name = rule.NetworkContextName.ToLower().Trim();
                if (!mJokers.Contains<string>(name))
                {
                    if (!string.IsNullOrEmpty(rule.Rule))
                    {
                        string ruleValue = rule.Rule.ToLower().Trim();
                        if ("*".Equals(ruleValue))
                        {
                            mJokers.Add(name);
                            if (mRules.ContainsKey(name))
                            {
                                mRules.Remove(name);
                            }
                        }
                        else if (!mJokers.Contains<string>(ruleValue))
                        {
                            HashSet<string> set = null;
                            if (mRules.ContainsKey(name))
                            {
                                set = mRules[name];
                            }
                            else
                            {
                                set = new HashSet<string>();
                                mRules.Add(name, set);
                            }
                            set.Add(ruleValue);

                            set = null;
                            if (mRules.ContainsKey(ruleValue))
                            {
                                set = mRules[ruleValue];
                            }
                            else
                            {
                                set = new HashSet<string>();
                                mRules.Add(ruleValue, set);
                            }
                            set.Add(name);
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal method(s)

        /// <summary>
        /// Check for separation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns>True, if the network context see each other. Otherwise false.</returns>
        internal bool CheckSeparation(string a, string b)
        {
            if (string.IsNullOrEmpty(a))
            {
                ThrowHelper.ThrowArgumentNullException("a");
            }
            if (string.IsNullOrEmpty(b))
            {
                ThrowHelper.ThrowArgumentNullException("b");
            }

            bool result = !mSeparation; // true, ha nincs szeparáció

            string valueA = a.ToLower().Trim();
            string valueB = b.ToLower().Trim();

            if (valueA.Equals(valueB))
            {
                result = true; // saját context-be mehet minden
            }
            else if (mJokers.Contains<string>(valueA) || mJokers.Contains<string>(valueB))
            {
                result = !result; // joker lista miatt tagadás
            }
            else
            {
                if (mRules.ContainsKey(valueA) && mRules[valueA].Contains<string>(valueB))
                {
                    result = !result;
                }
                else if (mRules.ContainsKey(valueB) && mRules[valueB].Contains<string>(valueA))
                {
                    result = !result;
                }
            }

            return result;
        }

        #endregion

    }

}
