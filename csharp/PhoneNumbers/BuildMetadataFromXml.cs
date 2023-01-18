﻿/*
 * Copyright (C) 2009 Google Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace PhoneNumbers
{
    /// <summary>
    /// The build metadata from xml.
    /// </summary>
    public class BuildMetadataFromXml
    {
        // String constants used to fetch the XML nodes and attributes.
        /// <summary>
        /// The CARRIE R COD E FORMATTIN G RULE.
        /// </summary>
        private const string CARRIER_CODE_FORMATTING_RULE = "carrierCodeFormattingRule";

        /// <summary>
        /// The CARRIE R SPECIFIC.
        /// </summary>
        private const string CARRIER_SPECIFIC = "carrierSpecific";
        /// <summary>
        /// The COUNTR Y CODE.
        /// </summary>
        private const string COUNTRY_CODE = "countryCode";
        /// <summary>
        /// The EMERGENCY.
        /// </summary>
        private const string EMERGENCY = "emergency";
        /// <summary>
        /// The EXAMPL E NUMBER.
        /// </summary>
        private const string EXAMPLE_NUMBER = "exampleNumber";
        /// <summary>
        /// The FIXE D LINE.
        /// </summary>
        private const string FIXED_LINE = "fixedLine";
        /// <summary>
        /// The FORMAT.
        /// </summary>
        private const string FORMAT = "format";
        /// <summary>
        /// The GENERA L DESC.
        /// </summary>
        private const string GENERAL_DESC = "generalDesc";
        /// <summary>
        /// The INTERNATIONA L PREFIX.
        /// </summary>
        private const string INTERNATIONAL_PREFIX = "internationalPrefix";
        /// <summary>
        /// The INT L FORMAT.
        /// </summary>
        private const string INTL_FORMAT = "intlFormat";
        /// <summary>
        /// The LEADIN G DIGITS.
        /// </summary>
        private const string LEADING_DIGITS = "leadingDigits";
        /// <summary>
        /// The MAI N COUNTR Y FO R CODE.
        /// </summary>
        private const string MAIN_COUNTRY_FOR_CODE = "mainCountryForCode";
        /// <summary>
        /// The MOBILE.
        /// </summary>
        private const string MOBILE = "mobile";
        /// <summary>
        /// The MOBIL E NUMBE R PORTABL E REGION.
        /// </summary>
        private const string MOBILE_NUMBER_PORTABLE_REGION = "mobileNumberPortableRegion";
        /// <summary>
        /// The NATIONA L NUMBE R PATTERN.
        /// </summary>
        private const string NATIONAL_NUMBER_PATTERN = "nationalNumberPattern";
        /// <summary>
        /// The NATIONA L PREFIX.
        /// </summary>
        private const string NATIONAL_PREFIX = "nationalPrefix";
        /// <summary>
        /// The NATIONA L PREFI X FORMATTIN G RULE.
        /// </summary>
        private const string NATIONAL_PREFIX_FORMATTING_RULE = "nationalPrefixFormattingRule";

        /// <summary>
        /// The NATIONA L PREFI X OPTIONA L WHE N FORMATTING.
        /// </summary>
        private const string NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING =
            "nationalPrefixOptionalWhenFormatting";

        /// <summary>
        /// The NATIONA L PREFI X FO R PARSING.
        /// </summary>
        private const string NATIONAL_PREFIX_FOR_PARSING = "nationalPrefixForParsing";
        /// <summary>
        /// The NATIONA L PREFI X TRANSFOR M RULE.
        /// </summary>
        private const string NATIONAL_PREFIX_TRANSFORM_RULE = "nationalPrefixTransformRule";
        /// <summary>
        /// The N O INTERNATIONA L DIALLING.
        /// </summary>
        private const string NO_INTERNATIONAL_DIALLING = "noInternationalDialling";
        /// <summary>
        /// The NUMBE R FORMAT.
        /// </summary>
        private const string NUMBER_FORMAT = "numberFormat";
        /// <summary>
        /// The PAGER.
        /// </summary>
        private const string PAGER = "pager";
        /// <summary>
        /// The PATTERN.
        /// </summary>
        private const string PATTERN = "pattern";
        /// <summary>
        /// The PERSONA L NUMBER.
        /// </summary>
        private const string PERSONAL_NUMBER = "personalNumber";
        /// <summary>
        /// The POSSIBL E LENGTHS.
        /// </summary>
        private const string POSSIBLE_LENGTHS = "possibleLengths";
        /// <summary>
        /// The NATIONAL.
        /// </summary>
        private const string NATIONAL = "national";
        /// <summary>
        /// The LOCA L ONLY.
        /// </summary>
        private const string LOCAL_ONLY = "localOnly";
        /// <summary>
        /// The PREFERRE D EXT N PREFIX.
        /// </summary>
        private const string PREFERRED_EXTN_PREFIX = "preferredExtnPrefix";
        /// <summary>
        /// The PREFERRE D INTERNATIONA L PREFIX.
        /// </summary>
        private const string PREFERRED_INTERNATIONAL_PREFIX = "preferredInternationalPrefix";
        /// <summary>
        /// The PREMIU M RATE.
        /// </summary>
        private const string PREMIUM_RATE = "premiumRate";
        /// <summary>
        /// The SHARE D COST.
        /// </summary>
        private const string SHARED_COST = "sharedCost";
        /// <summary>
        /// The SHOR T CODE.
        /// </summary>
        private const string SHORT_CODE = "shortCode";
        /// <summary>
        /// The SM S SERVICES.
        /// </summary>
        private const string SMS_SERVICES = "smsServices";
        /// <summary>
        /// The STANDAR D RATE.
        /// </summary>
        private const string STANDARD_RATE = "standardRate";
        /// <summary>
        /// The TOL L FREE.
        /// </summary>
        private const string TOLL_FREE = "tollFree";
        /// <summary>
        /// The UAN.
        /// </summary>
        private const string UAN = "uan";
        /// <summary>
        /// The VOICEMAIL.
        /// </summary>
        private const string VOICEMAIL = "voicemail";
        /// <summary>
        /// The VOIP.
        /// </summary>
        private const string VOIP = "voip";

        // Build the PhoneMetadataCollection from the input XML file.
        /// <summary>
        /// Build the phone metadata collection.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="liteBuild">If true, lite build.</param>
        /// <param name="specialBuild">If true, special build.</param>
        /// <param name="isShortNumberMetadata">If true, is short number metadata.</param>
        /// <param name="isAlternateFormatsMetadata">If true, is alternate formats metadata.</param>
        /// <returns>A PhoneMetadataCollection.</returns>
        public static PhoneMetadataCollection BuildPhoneMetadataCollection(string name, bool liteBuild, bool specialBuild, bool isShortNumberMetadata, bool isAlternateFormatsMetadata)
            => BuildPhoneMetadata(name, null, liteBuild, specialBuild, isShortNumberMetadata, isAlternateFormatsMetadata, nameSuffix: false);

        /// <summary>
        /// Build the phone metadata.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="asm">The asm.</param>
        /// <param name="liteBuild">If true, lite build.</param>
        /// <param name="specialBuild">If true, special build.</param>
        /// <param name="isShortNumberMetadata">If true, is short number metadata.</param>
        /// <param name="isAlternateFormatsMetadata">If true, is alternate formats metadata.</param>
        /// <param name="nameSuffix">If true, name suffix.</param>
        /// <returns>A PhoneMetadataCollection.</returns>
        internal static PhoneMetadataCollection BuildPhoneMetadata(string name, Assembly asm = null,
            bool liteBuild = false, bool specialBuild = false, bool isShortNumberMetadata = false,
            bool isAlternateFormatsMetadata = false,
            bool nameSuffix = true)
        {
            using var input = GetStream(name, asm, nameSuffix);
            return BuildPhoneMetadataFromStream(input, liteBuild, specialBuild, isShortNumberMetadata,
                isAlternateFormatsMetadata);
        }

        /// <summary>
        /// Gets the stream.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="asm">The asm.</param>
        /// <param name="nameSuffix">If true, name suffix.</param>
        /// <exception cref="ArgumentException"></exception>
        /// <returns>A Stream.</returns>
        internal static Stream GetStream(string name, Assembly asm = null, bool nameSuffix = true)
        {
            asm ??= typeof(PhoneNumberUtil).GetAssembly();
            if (nameSuffix)
                name = asm.GetManifestResourceNames().FirstOrDefault(n => n.EndsWith(name, Ordinal)) ??
                       throw new ArgumentException(name + " resource not found");

            return asm.GetManifestResourceStream(name);
        }

        /// <summary>
        /// Build the phone metadata from stream.
        /// </summary>
        /// <param name="metadataStream">The metadata stream.</param>
        /// <param name="liteBuild">If true, lite build.</param>
        /// <param name="specialBuild">If true, special build.</param>
        /// <param name="isShortNumberMetadata">If true, is short number metadata.</param>
        /// <param name="isAlternateFormatsMetadata">If true, is alternate formats metadata.</param>
        /// <returns>A PhoneMetadataCollection.</returns>
        internal static PhoneMetadataCollection BuildPhoneMetadataFromStream(Stream metadataStream,
            bool liteBuild = false, bool specialBuild = false, bool isShortNumberMetadata = false,
            bool isAlternateFormatsMetadata = false)
        {
            var document = XDocument.Load(metadataStream);

            var metadataCollection = new PhoneMetadataCollection.Builder();
            var metadataFilter = GetMetadataFilter(liteBuild, specialBuild);
            foreach (var territory in document.Root.Element("territories").Elements())
            {
                // For the main metadata file this should always be set, but for other supplementary data
                // files the country calling code may be all that is needed.
                var regionCode = territory.GetAttribute("id");
                var metadata = LoadCountryMetadata(regionCode, territory,
                    isShortNumberMetadata, isAlternateFormatsMetadata);
                metadataFilter.FilterMetadata(metadata);
                metadataCollection.AddMetadata(metadata);
            }

            return metadataCollection.Build();
        }

        // Build a mapping from a country calling code to the region codes which denote the country/region
        // represented by that country code. In the case of multiple countries sharing a calling code,
        // such as the NANPA countries, the one indicated with "isMainCountryForCode" in the metadata
        // should be first.
        /// <summary>
        /// Builds the country code to region code map.
        /// </summary>
        /// <param name="metadataCollection">The metadata collection.</param>
        /// <returns><![CDATA[Dictionary<int, List<string>>]]></returns>
        public static Dictionary<int, List<string>> BuildCountryCodeToRegionCodeMap(
            PhoneMetadataCollection metadataCollection)
        {
            var countryCodeToRegionCodeMap =
                new Dictionary<int, List<string>>();
            foreach (var metadata in metadataCollection.MetadataList)
            {
                var regionCode = metadata.Id;
                var countryCode = metadata.CountryCode;
                if (countryCodeToRegionCodeMap.TryGetValue(countryCode, out var list))
                {
                    if (metadata.MainCountryForCode)
                        list.Insert(0, regionCode);
                    else
                        list.Add(regionCode);
                }
                else
                {
                    // For most countries, there will be only one region code for the country calling code.
                    var listWithRegionCode = new List<string>(1);
                    if (regionCode.Length > 0)
                        listWithRegionCode.Add(regionCode);
                    countryCodeToRegionCodeMap[countryCode] = listWithRegionCode;
                }
            }
            return countryCodeToRegionCodeMap;
        }

        /// <summary>
        /// The valid patterns.
        /// </summary>
        private static readonly HashSet<string> ValidPatterns = new HashSet<string>();

        /// <summary>
        /// Validates the RE.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <returns>A string.</returns>
        public static string ValidateRE(string regex) => ValidateRE(regex, false);

        /// <summary>
        /// Validates the RE.
        /// </summary>
        /// <param name="regex">The regex.</param>
        /// <param name="removeWhitespace">If true, remove whitespace.</param>
        /// <returns>A string.</returns>
        public static string ValidateRE(string regex, bool removeWhitespace)
        {
            // Removes all the whitespace and newline from the regexp. Not using pattern compile options to
            // make it work across programming languages.
            if (removeWhitespace)
            {
                for (int i = 0; i < regex.Length; i++)
                    if (char.IsWhiteSpace(regex[i]))
                    {
                        var sb = new StringBuilder(regex, 0, i, regex.Length);
                        while (++i < regex.Length)
                        {
                            if (!char.IsWhiteSpace(regex[i]))
                                sb.Append(regex[i]);
                        }
                        regex = sb.ToString();
                        break;
                    }
            }

            lock (ValidPatterns)
                if (!ValidPatterns.Contains(regex))
                {
#pragma warning disable S1848
                    // ReSharper disable once ObjectCreationAsStatement
                    new Regex(regex, RegexOptions.CultureInvariant);
#pragma warning restore S1848
                    ValidPatterns.Add(regex);
                }
            // return regex itself if it is of correct regex syntax
            // i.e. compile did not fail with a PatternSyntaxException.
            return regex;
        }

        /// <summary>
        /// Returns <inheritdoc cref="GetNationalPrefix(XElement)" path="/returns" />.
        /// </summary>
        /// <param name="element">The country element.</param>
        /// <returns>the national prefix of the provided country element.</returns>
        public static string GetNationalPrefix(XElement element)
        {
            return element.GetAttribute(NATIONAL_PREFIX);
        }

        /// <summary>
        /// Load territory tag metadata.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="element">The element.</param>
        /// <param name="nationalPrefix">The national prefix.</param>
        /// <returns>A PhoneMetadata.Builder.</returns>
        public static PhoneMetadata.Builder LoadTerritoryTagMetadata(string regionCode, XElement element,
            string nationalPrefix)
        {
            var metadata = new PhoneMetadata.Builder();
            metadata.SetId(regionCode);
            if (element.HasAttribute(COUNTRY_CODE))
                metadata.SetCountryCode(int.Parse(element.GetAttribute(COUNTRY_CODE)));
            if (element.HasAttribute(LEADING_DIGITS))
                metadata.SetLeadingDigits(ValidateRE(element.GetAttribute(LEADING_DIGITS)));
            if (element.HasAttribute(INTERNATIONAL_PREFIX))
                metadata.SetInternationalPrefix(ValidateRE(element.GetAttribute(INTERNATIONAL_PREFIX)));
            if (element.HasAttribute(PREFERRED_INTERNATIONAL_PREFIX))
                metadata.SetPreferredInternationalPrefix(
                    element.GetAttribute(PREFERRED_INTERNATIONAL_PREFIX));
            if (element.HasAttribute(NATIONAL_PREFIX_FOR_PARSING))
            {
                metadata.SetNationalPrefixForParsing(
                    ValidateRE(element.GetAttribute(NATIONAL_PREFIX_FOR_PARSING), true));
                if (element.HasAttribute(NATIONAL_PREFIX_TRANSFORM_RULE))
                    metadata.SetNationalPrefixTransformRule(
                        ValidateRE(element.GetAttribute(NATIONAL_PREFIX_TRANSFORM_RULE)));
            }
            if (!string.IsNullOrEmpty(nationalPrefix))
            {
                metadata.SetNationalPrefix(nationalPrefix);
                if (!metadata.HasNationalPrefixForParsing)
                    metadata.SetNationalPrefixForParsing(nationalPrefix);
            }
            if (element.HasAttribute(PREFERRED_EXTN_PREFIX))
                metadata.SetPreferredExtnPrefix(element.GetAttribute(PREFERRED_EXTN_PREFIX));
            if (element.HasAttribute(MAIN_COUNTRY_FOR_CODE))
                metadata.SetMainCountryForCode(true);
            if (element.HasAttribute(MOBILE_NUMBER_PORTABLE_REGION))
                metadata.SetMobileNumberPortableRegion(true);
            return metadata;
        }

        /// <summary>
        /// Extracts the pattern for international format. If there is no intlFormat, default to using the
        /// national format. If the intlFormat is set to "NA" the intlFormat should be ignored.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="numberFormatElement">The number format element.</param>
        /// <param name="nationalFormat">The national format.</param>
        /// <exception cref="Exception">RuntimeException if multiple intlFormats have been encountered</exception>
        /// <returns>whether an international number format is defined.</returns>
        public static bool LoadInternationalFormat(PhoneMetadata.Builder metadata,
            XElement numberFormatElement,
            string nationalFormat)
        {
            var intlFormat = new NumberFormat.Builder();
            SetLeadingDigitsPatterns(numberFormatElement, intlFormat);
            intlFormat.SetPattern(numberFormatElement.GetAttribute(PATTERN));
            var intlFormatPattern = numberFormatElement.Elements(INTL_FORMAT).ToList();
            var hasExplicitIntlFormatDefined = false;

            if (intlFormatPattern.Count > 1)
                throw new Exception("Invalid number of intlFormat patterns for country: " +
                                    metadata.Id);
            if (intlFormatPattern.Count == 0)
            {
                // Default to use the same as the national pattern if none is defined.
                intlFormat.SetFormat(nationalFormat);
            }
            else
            {
                var intlFormatPatternValue = intlFormatPattern.First().Value;
                intlFormat.SetFormat(intlFormatPatternValue);
                hasExplicitIntlFormatDefined = true;
            }

            if (intlFormat.HasFormat)
                metadata.AddIntlNumberFormat(intlFormat);
            return hasExplicitIntlFormatDefined;
        }

        /**
         * Extracts the pattern for the national format.
         *
         * @throws  RuntimeException if multiple or no formats have been encountered.
         * @return  the national format string.
         */
        public static string LoadNationalFormat(PhoneMetadata.Builder metadata, XElement numberFormatElement,
            NumberFormat.Builder format)
        {
            SetLeadingDigitsPatterns(numberFormatElement, format);
            format.SetPattern(ValidateRE(numberFormatElement.GetAttribute(PATTERN)));

            var formatPattern = numberFormatElement.Elements(FORMAT).ToList();
            if (formatPattern.Count != 1)
                throw new Exception("Invalid number of format patterns for country: " +
                                    metadata.Id);
            var nationalFormat = formatPattern[0].Value;
            format.SetFormat(nationalFormat);
            return nationalFormat;
        }

        /**
         * <summary>
         *  Extracts the available formats from the provided DOM element. If it does not contain any
         *  nationalPrefixFormattingRule, the one passed-in is retained. The nationalPrefix,
         *  nationalPrefixFormattingRule and nationalPrefixOptionalWhenFormatting values are provided from
         *  the parent (territory) element.
         *  </summary>
         */
        /// <param name="metadata">The metadata.</param>
        /// <param name="element">The element.</param>
        /// <param name="nationalPrefix">The national prefix.</param>
        /// <param name="nationalPrefixFormattingRule">The national prefix formatting rule.</param>
        /// <param name="nationalPrefixOptionalWhenFormatting">If true, national prefix optional when formatting.</param>
        public static void LoadAvailableFormats(PhoneMetadata.Builder metadata,
            XElement element, string nationalPrefix,
            string nationalPrefixFormattingRule,
            bool nationalPrefixOptionalWhenFormatting)
        {
            var carrierCodeFormattingRule = "";
            if (element.HasAttribute(CARRIER_CODE_FORMATTING_RULE))
                carrierCodeFormattingRule = ValidateRE(
                    GetDomesticCarrierCodeFormattingRuleFromElement(element, nationalPrefix));

            var availableFormats = element.Element("availableFormats");
            var hasExplicitIntlFormatDefined = false;

            if (availableFormats != null && availableFormats.HasElements)
            {
                foreach (var numberFormatElement in availableFormats.Elements())
                {
                    var format = new NumberFormat.Builder();

                    if (numberFormatElement.HasAttribute(NATIONAL_PREFIX_FORMATTING_RULE))
                    {
                        format.SetNationalPrefixFormattingRule(
                            GetNationalPrefixFormattingRuleFromElement(numberFormatElement, nationalPrefix));
                    }
                    else if (!nationalPrefixFormattingRule.Equals(""))
                    {
                        format.SetNationalPrefixFormattingRule(nationalPrefixFormattingRule);
                    }

                    if (numberFormatElement.HasAttribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING))
                        format.SetNationalPrefixOptionalWhenFormatting(
                            bool.Parse(numberFormatElement.Attribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING).Value));
                    else if (format.NationalPrefixOptionalWhenFormatting
                             != nationalPrefixOptionalWhenFormatting)
                    {
                        // Inherit from the parent field if it is not already the same as the default.
                        format.SetNationalPrefixOptionalWhenFormatting(nationalPrefixOptionalWhenFormatting);
                    }
                    if (numberFormatElement.HasAttribute("carrierCodeFormattingRule"))
                        format.SetDomesticCarrierCodeFormattingRule(ValidateRE(
                            GetDomesticCarrierCodeFormattingRuleFromElement(
                                numberFormatElement, nationalPrefix)));
                    else if (!carrierCodeFormattingRule.Equals(""))
                        format.SetDomesticCarrierCodeFormattingRule(carrierCodeFormattingRule);

                    // Extract the pattern for the national format.
                    var nationalFormat =
                        LoadNationalFormat(metadata, numberFormatElement, format);
                    metadata.AddNumberFormat(format);

                    if (LoadInternationalFormat(metadata, numberFormatElement, nationalFormat))
                        hasExplicitIntlFormatDefined = true;
                }
                // Only a small number of regions need to specify the intlFormats in the xml. For the majority
                // of countries the intlNumberFormat metadata is an exact copy of the national NumberFormat
                // metadata. To minimize the size of the metadata file, we only keep intlNumberFormats that
                // actually differ in some way to the national formats.
                if (!hasExplicitIntlFormatDefined)
                    metadata.ClearIntlNumberFormat();
            }
        }

        /// <summary>
        /// Sets the leading digits patterns.
        /// </summary>
        /// <param name="numberFormatElement">The number format element.</param>
        /// <param name="format">The format.</param>
        public static void SetLeadingDigitsPatterns(XElement numberFormatElement, NumberFormat.Builder format)
        {
            foreach (var e in numberFormatElement.Elements(LEADING_DIGITS))
                format.AddLeadingDigitsPattern(ValidateRE(e.Value, true));
        }

        /// <summary>
        /// Gets the national prefix formatting rule from element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="nationalPrefix">The national prefix.</param>
        /// <returns>A string.</returns>
        public static string GetNationalPrefixFormattingRuleFromElement(XElement element,
            string nationalPrefix)
        {
            var nationalPrefixFormattingRule = element.GetAttribute(NATIONAL_PREFIX_FORMATTING_RULE);
            // Replace $NP with national prefix and $FG with the first group ($1).
            nationalPrefixFormattingRule = ReplaceFirst(nationalPrefixFormattingRule, "$NP", nationalPrefix);
            nationalPrefixFormattingRule = ReplaceFirst(nationalPrefixFormattingRule, "$FG", "${1}");
            return nationalPrefixFormattingRule;
        }

        /// <summary>
        /// Gets the domestic carrier code formatting rule from element.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="nationalPrefix">The national prefix.</param>
        /// <returns>A string.</returns>
        public static string GetDomesticCarrierCodeFormattingRuleFromElement(XElement element,
            string nationalPrefix)
        {
            var carrierCodeFormattingRule = element.GetAttribute(CARRIER_CODE_FORMATTING_RULE);
            // Replace $FG with the first group ($1) and $NP with the national prefix.
            carrierCodeFormattingRule = ReplaceFirst(carrierCodeFormattingRule, "$FG", "${1}");
            carrierCodeFormattingRule = ReplaceFirst(carrierCodeFormattingRule, "$NP", nationalPrefix);
            return carrierCodeFormattingRule;
        }

        /**
        * Checks if the possible lengths provided as a sorted set are equal to the possible lengths
        * stored already in the description pattern. Note that possibleLengths may be empty but must not
        * be null, and the PhoneNumberDesc passed in should also not be null.
        */
        private static bool ArePossibleLengthsEqual(SortedSet<int> possibleLengths,
            PhoneNumberDesc desc)
        {
            if (possibleLengths.Count != desc.PossibleLengthCount)
                return false;
            // Note that both should be sorted already, and we know they are the same length.
            var i = 0;
            foreach (var length in possibleLengths)
            {
                if (length != desc.PossibleLengthList[i])
                    return false;
                i++;
            }
            return true;
        }

        /**
        * Processes a phone number description element from the XML file and returns it as a
        * PhoneNumberDesc. If the description element is a fixed line or mobile number, the parent
        * description will be used to fill in the whole element if necessary, or any components that are
        * missing. For all other types, the parent description will only be used to fill in missing
        * components if the type has a partial definition. For example, if no "tollFree" element exists,
        * we assume there are no toll free numbers for that locale, and return a phone number description
        * with no national number data and [-1] for the possible lengths. Note that the parent
        * description must therefore already be processed before this method is called on any child
        * elements.
        *
        * @param generalDesc  a generic phone number description that will be used to fill in missing
        *                     parts of the description
        * @param countryElement  the XML element representing all the country information
        * @param numberType  the name of the number type, corresponding to the appropriate tag in the XML
        *                    file with information about that type
        * @return  complete description of that phone number type
        */
        public static PhoneNumberDesc.Builder ProcessPhoneNumberDescElement(PhoneNumberDesc parentDesc,
            XElement countryElement, string numberType)
        {
            var phoneNumberDescList = countryElement.Elements(numberType).ToList();
            var numberDesc = new PhoneNumberDesc.Builder();
            if (phoneNumberDescList.Count == 0)
            {
                // -1 will never match a possible phone number length, so is safe to use to ensure this never
                // matches. We don't leave it empty, since for compression reasons, we use the empty list to
                // mean that the generalDesc possible lengths apply.
                numberDesc.AddPossibleLength(-1);
                return numberDesc;
            }
            if (phoneNumberDescList.Count > 1)
                throw new Exception($"Multiple elements with type {numberType} found.");
            var element = phoneNumberDescList[0];

            parentDesc ??= new PhoneNumberDesc();
            var lengths = new SortedSet<int>();
            var localOnlyLengths = new SortedSet<int>();
            PopulatePossibleLengthSets(element.Elements(POSSIBLE_LENGTHS), lengths, localOnlyLengths);
            SetPossibleLengths(lengths, localOnlyLengths, parentDesc, numberDesc);

            var validPattern = element.Element(NATIONAL_NUMBER_PATTERN);
            if (validPattern != null)
                numberDesc.SetNationalNumberPattern(ValidateRE(validPattern.Value, true));

            var exampleNumber = element.Element(EXAMPLE_NUMBER);
            if (exampleNumber != null)
                numberDesc.SetExampleNumber(exampleNumber.Value);

            return numberDesc;
        }

        /// <summary>
        /// Sets the relevant desc patterns.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="element">The element.</param>
        /// <param name="isShortNumberMetadata">If true, is short number metadata.</param>
        private static void SetRelevantDescPatterns(PhoneMetadata.Builder metadata, XElement element,
            bool isShortNumberMetadata)
        {
            var generalDescBuilder = ProcessPhoneNumberDescElement(null, element,
                GENERAL_DESC);
            // Calculate the possible lengths for the general description. This will be based on the
            // possible lengths of the child elements.
            SetPossibleLengthsGeneralDesc(
                generalDescBuilder, metadata.Id, element, isShortNumberMetadata);
            metadata.SetGeneralDesc(generalDescBuilder);

            var generalDesc = metadata.GeneralDesc;

            if (!isShortNumberMetadata)
            {
                // Set fields used by regular length phone numbers.
                metadata.SetFixedLine(ProcessPhoneNumberDescElement(generalDesc, element, FIXED_LINE));
                metadata.SetMobile(ProcessPhoneNumberDescElement(generalDesc, element, MOBILE));
                metadata.SetSharedCost(ProcessPhoneNumberDescElement(generalDesc, element, SHARED_COST));
                metadata.SetVoip(ProcessPhoneNumberDescElement(generalDesc, element, VOIP));
                metadata.SetPersonalNumber(ProcessPhoneNumberDescElement(generalDesc, element,
                    PERSONAL_NUMBER));
                metadata.SetPager(ProcessPhoneNumberDescElement(generalDesc, element, PAGER));
                metadata.SetUan(ProcessPhoneNumberDescElement(generalDesc, element, UAN));
                metadata.SetVoicemail(ProcessPhoneNumberDescElement(generalDesc, element, VOICEMAIL));
                metadata.SetNoInternationalDialling(ProcessPhoneNumberDescElement(generalDesc, element,
                    NO_INTERNATIONAL_DIALLING));
                var mobileAndFixedAreSame = metadata.Mobile.NationalNumberPattern
                    .Equals(metadata.FixedLine.NationalNumberPattern);
                if (metadata.SameMobileAndFixedLinePattern != mobileAndFixedAreSame)
                    metadata.SetSameMobileAndFixedLinePattern(mobileAndFixedAreSame);
                metadata.SetTollFree(ProcessPhoneNumberDescElement(generalDesc, element, TOLL_FREE));
                metadata.SetPremiumRate(ProcessPhoneNumberDescElement(generalDesc, element, PREMIUM_RATE));
            }
            else
            {
                // Set fields used by short numbers.
                metadata.SetStandardRate(ProcessPhoneNumberDescElement(generalDesc, element, STANDARD_RATE));
                metadata.SetShortCode(ProcessPhoneNumberDescElement(generalDesc, element, SHORT_CODE));
                metadata.SetCarrierSpecific(ProcessPhoneNumberDescElement(generalDesc, element,
                    CARRIER_SPECIFIC));
                metadata.SetEmergency(ProcessPhoneNumberDescElement(generalDesc, element, EMERGENCY));
                metadata.SetTollFree(ProcessPhoneNumberDescElement(generalDesc, element, TOLL_FREE));
                metadata.SetPremiumRate(ProcessPhoneNumberDescElement(generalDesc, element, PREMIUM_RATE));
                metadata.SetSmsServices(ProcessPhoneNumberDescElement(generalDesc, element, SMS_SERVICES));
            }
        }

        /// <summary>
        /// Parses the possible length string to set.
        /// </summary>
        /// <param name="possibleLengthString">The possible length string.</param>
        /// <exception cref="Exception"></exception>
        /// <returns><![CDATA[SortedSet<int>]]></returns>
        private static SortedSet<int> ParsePossibleLengthStringToSet(string possibleLengthString)
        {
            if (possibleLengthString.Length == 0)
                throw new Exception("Empty possibleLength string found.");
            var lengths = possibleLengthString.Split(',');
            var lengthSet = new SortedSet<int>();
            foreach (var lengthSubstring in lengths)
            {
                if (lengthSubstring.Length == 0)
                    throw new Exception("Leading, trailing or adjacent commas in possible " +
                                        $"length string {possibleLengthString}, these should only separate numbers or ranges.");
                if (lengthSubstring[0] == '[')
                {
                    if (lengthSubstring[lengthSubstring.Length - 1] != ']')
                        throw new Exception("Missing end of range character in possible " +
                                            $"length string {possibleLengthString}.");
                    // Strip the leading and trailing [], and split on the -.
                    var minMax = lengthSubstring.Substring(1, lengthSubstring.Length - 2).Split('-');
                    if (minMax.Length != 2)
                        throw new Exception("Ranges must have exactly one - character: " +
                                            $"missing for {possibleLengthString}.");
                    var min = int.Parse(minMax[0]);
                    var max = int.Parse(minMax[1]);
                    // We don't even accept [6-7] since we prefer the shorter 6,7 variant; for a range to be in
                    // use the hyphen needs to replace at least one digit.
                    if (max - min < 2)
                        throw new Exception("The first number in a range should be two or " +
                                            $"more digits lower than the second. Culprit possibleLength string: {possibleLengthString}");
                    for (var j = min; j <= max; j++)
                        if (!lengthSet.Add(j))
                            throw new Exception($"Duplicate length element found ({j}) in " +
                                                $"possibleLength string {possibleLengthString}");
                }
                else
                {
                    var length = int.Parse(lengthSubstring);
                    if (!lengthSet.Add(length))
                        throw new Exception($"Duplicate length element found ({length}) in " +
                                            $"possibleLength string {possibleLengthString}");
                }
            }
            return lengthSet;
        }

        /**
         * Reads the possible lengths present in the metadata and splits them into two sets: one for
         * full-length numbers, one for local numbers.
         *
         * @param data  one or more phone number descriptions, represented as XML nodes
         * @param lengths  a set to which to add possible lengths of full phone numbers
         * @param localOnlyLengths  a set to which to add possible lengths of phone numbers only diallable
         *     locally (e.g. within a province)
         */
        private static void PopulatePossibleLengthSets(IEnumerable<XElement> possibleLengths, SortedSet<int> lengths,
            SortedSet<int> localOnlyLengths)
        {
            foreach (var element in possibleLengths)
            {
                var nationalLengths = element.GetAttribute(NATIONAL);
                // We don't add to the phone metadata yet, since we want to sort length elements found under
                // different nodes first, make sure there are no duplicates between them and that the
                // localOnly lengths don't overlap with the others.
                var thisElementLengths = ParsePossibleLengthStringToSet(nationalLengths);
                if (element.Attribute(LOCAL_ONLY) is { } localLengths)
                {
                    var thisElementLocalOnlyLengths = ParsePossibleLengthStringToSet(localLengths.Value);
                    if (thisElementLengths.Overlaps(thisElementLocalOnlyLengths))
                        throw new Exception(
                            $"Possible length(s) found specified as a normal and local-only length: {thisElementLengths.Intersect(thisElementLocalOnlyLengths)}");
                    // We check again when we set these lengths on the metadata itself in setPossibleLengths
                    // that the elements in localOnly are not also in lengths. For e.g. the generalDesc, it
                    // might have a local-only length for one type that is a normal length for another type. We
                    // don't consider this an error, but we do want to remove the local-only lengths.
                    foreach (var length in thisElementLocalOnlyLengths)
                        localOnlyLengths.Add(length);
                }
                // It is okay if at this time we have duplicates, because the same length might be possible
                // for e.g. fixed-line and for mobile numbers, and this method operates potentially on
                // multiple phoneNumberDesc XML elements.
                foreach (var length in thisElementLengths)
                    lengths.Add(length);
            }
        }

        /**
         * Sets possible lengths in the general description, derived from certain child elements.
         */
        private static void SetPossibleLengthsGeneralDesc(PhoneNumberDesc.Builder generalDesc, string metadataId,
            XElement data, bool isShortNumberMetadata)
        {
            var lengths = new SortedSet<int>();
            var localOnlyLengths = new SortedSet<int>();
            // The general description node should *always* be present if metadata for other types is
            // present, aside from in some unit tests.
            // (However, for e.g. formatting metadata in PhoneNumberAlternateFormats, no PhoneNumberDesc
            // elements are present).
            var generalDescNode = data.Element(GENERAL_DESC);
            if (generalDescNode != null)
            {
                PopulatePossibleLengthSets(generalDescNode.Elements(POSSIBLE_LENGTHS), lengths, localOnlyLengths);
                if (lengths.Count != 0 || localOnlyLengths.Count != 0)
                    throw new Exception("Found possible lengths specified at general " +
                                        $"desc: this should be derived from child elements. Affected country: {metadataId}");
            }
            if (!isShortNumberMetadata)
            {
                var allDescData = data.Descendants(POSSIBLE_LENGTHS).Where(e => e.Parent.Name != NO_INTERNATIONAL_DIALLING);
                PopulatePossibleLengthSets(allDescData, lengths, localOnlyLengths);
            }
            else
            {
                // For short number metadata, we want to copy the lengths from the "short code" section only.
                // This is because it's the more detailed validation pattern, it's not a sub-type of short
                // codes. The other lengths will be checked later to see that they are a sub-set of these
                // possible lengths.
                var shortCodeDesc = data.Element(SHORT_CODE);
                if (shortCodeDesc != null)
                {
                    PopulatePossibleLengthSets(shortCodeDesc.Elements(POSSIBLE_LENGTHS), lengths, localOnlyLengths);
                }
                if (localOnlyLengths.Count > 0)
                    throw new Exception("Found local-only lengths in short-number metadata");
            }
            SetPossibleLengths(lengths, localOnlyLengths, null, generalDesc);
        }

        /**
        * Sets the possible length fields in the metadata from the sets of data passed in. Checks that
        * the length is covered by the "parent" phone number description element if one is present, and
        * if the lengths are exactly the same as this, they are not filled in for efficiency reasons.
        *
        * @param parentDesc  the "general description" element or null if desc is the generalDesc itself
        * @param desc  the PhoneNumberDesc object that we are going to set lengths for
        */
        private static void SetPossibleLengths(SortedSet<int> lengths,
            SortedSet<int> localOnlyLengths, PhoneNumberDesc parentDesc, PhoneNumberDesc.Builder desc)
        {
            // Only add the lengths to this sub-type if they aren't exactly the same as the possible
            // lengths in the general desc (for metadata size reasons).
            if (parentDesc == null || !ArePossibleLengthsEqual(lengths, parentDesc))
                foreach (var length in lengths)
                    if (parentDesc == null || parentDesc.PossibleLengthList.Contains(length))
                        desc.PossibleLengthList.Add(length);
                    else
                        throw new Exception(
                            $"Out-of-range possible length found ({length}), parent lengths {string.Join(", ", parentDesc.PossibleLengthList)}.");
            // We check that the local-only length isn't also a normal possible length (only relevant for
            // the general-desc, since within elements such as fixed-line we would throw an exception if we
            // saw this) before adding it to the collection of possible local-only lengths.
            foreach (var length in localOnlyLengths)
                if (!lengths.Contains(length))
                    if (parentDesc == null || parentDesc.PossibleLengthLocalOnlyList.Contains(length)
                        || parentDesc.PossibleLengthList.Contains(length))
                        desc.PossibleLengthLocalOnlyList.Add(length);
                    else
                        throw new Exception(
                            $"Out-of-range local-only possible length found ({length}), parent length {string.Join(", ", parentDesc.PossibleLengthLocalOnlyList)}.");
        }


        /// <summary>
        /// Replace first.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="value">The value.</param>
        /// <param name="replacement">The replacement.</param>
        /// <returns>A string.</returns>
        private static string ReplaceFirst(string input, string value, string replacement)
        {
            var p = input.IndexOf(value, StringComparison.Ordinal);
            if (p >= 0)
                input = input.Substring(0, p) + replacement + input.Substring(p + value.Length);
            return input;
        }

        // @VisibleForTesting
        /// <summary>
        /// Load general desc.
        /// </summary>
        /// <param name="metadata">The metadata.</param>
        /// <param name="element">The element.</param>
        public static void LoadGeneralDesc(PhoneMetadata.Builder metadata, XElement element)
        {
            var generalDescBuilder = ProcessPhoneNumberDescElement(null, element, GENERAL_DESC);
            SetPossibleLengthsGeneralDesc(generalDescBuilder, metadata.Id, element, false);
            var generalDesc = generalDescBuilder.Build();

            metadata.SetFixedLine(ProcessPhoneNumberDescElement(generalDesc, element, FIXED_LINE));
            metadata.SetMobile(ProcessPhoneNumberDescElement(generalDesc, element, MOBILE));
            metadata.SetTollFree(ProcessPhoneNumberDescElement(generalDesc, element, TOLL_FREE));
            metadata.SetPremiumRate(ProcessPhoneNumberDescElement(generalDesc, element, PREMIUM_RATE));
            metadata.SetSharedCost(ProcessPhoneNumberDescElement(generalDesc, element, SHARED_COST));
            metadata.SetVoip(ProcessPhoneNumberDescElement(generalDesc, element, VOIP));
            metadata.SetPersonalNumber(ProcessPhoneNumberDescElement(generalDesc, element, PERSONAL_NUMBER));
            metadata.SetPager(ProcessPhoneNumberDescElement(generalDesc, element, PAGER));
            metadata.SetUan(ProcessPhoneNumberDescElement(generalDesc, element, UAN));
            metadata.SetVoicemail(ProcessPhoneNumberDescElement(generalDesc, element, VOICEMAIL));
            metadata.SetEmergency(ProcessPhoneNumberDescElement(generalDesc, element, EMERGENCY));
            metadata.SetNoInternationalDialling(
                ProcessPhoneNumberDescElement(generalDesc, element, NO_INTERNATIONAL_DIALLING));
            metadata.SetSameMobileAndFixedLinePattern(
                metadata.Mobile.NationalNumberPattern.Equals(
                    metadata.FixedLine.NationalNumberPattern));
        }

        /// <summary>
        /// Load country metadata.
        /// </summary>
        /// <param name="regionCode">The region code.</param>
        /// <param name="element">The element.</param>
        /// <param name="isShortNumberMetadata">If true, is short number metadata.</param>
        /// <param name="isAlternateFormatsMetadata">If true, is alternate formats metadata.</param>
        /// <returns>A PhoneMetadata.Builder.</returns>
        public static PhoneMetadata.Builder LoadCountryMetadata(string regionCode,
            XElement element,
            bool isShortNumberMetadata,
            bool isAlternateFormatsMetadata)
        {
            var nationalPrefix = GetNationalPrefix(element);
            var metadata = LoadTerritoryTagMetadata(regionCode, element, nationalPrefix);
            var nationalPrefixFormattingRule = GetNationalPrefixFormattingRuleFromElement(element, nationalPrefix);
            LoadAvailableFormats(metadata, element, nationalPrefix,
                nationalPrefixFormattingRule,
                element.HasAttribute(NATIONAL_PREFIX_OPTIONAL_WHEN_FORMATTING));
            LoadGeneralDesc(metadata, element);
            if (!isAlternateFormatsMetadata)
                SetRelevantDescPatterns(metadata, element, isShortNumberMetadata);
            return metadata;
        }

        /// <summary>
        /// Gets the country code to region code map.
        /// </summary>
        /// <param name="filePrefix">The file prefix.</param>
        /// <returns><![CDATA[Dictionary<int, List<string>>]]></returns>
        public static Dictionary<int, List<string>> GetCountryCodeToRegionCodeMap(string filePrefix)
        {
            var collection = BuildPhoneMetadata(filePrefix); // todo lite/special build
            return BuildCountryCodeToRegionCodeMap(collection);
        }

        /**
         * Processes the custom build flags and gets a {@code MetadataFilter} which may be used to
        * filter {@code PhoneMetadata} objects. Incompatible flag combinations throw RuntimeException.
        *
        * @param liteBuild  The liteBuild flag value as given by the command-line
        * @param specialBuild  The specialBuild flag value as given by the command-line
        */
        // @VisibleForTesting
        internal static MetadataFilter GetMetadataFilter(bool liteBuild, bool specialBuild)
        {
            if (specialBuild)
            {
                if (liteBuild)
                    throw new Exception("liteBuild and specialBuild may not both be set");
                return MetadataFilter.ForSpecialBuild();
            }
            return liteBuild ? MetadataFilter.ForLiteBuild() : MetadataFilter.EmptyFilter();
        }
    }
}
