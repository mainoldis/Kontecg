using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Kontecg.Collections.Extensions;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;

namespace Kontecg.Organizations
{
    /// <summary>
    ///     Represents an organization unit (OU).
    /// </summary>
    [Table("organization_units", Schema = "est")]
    public class OrganizationUnit : FullAuditedEntity<long>, IMayHaveCompany
    {
        /// <summary>
        ///     Maximum length of the <see cref="DisplayName" /> property.
        /// </summary>
        public const int MaxDisplayNameLength = 128;

        /// <summary>
        ///     Maximum depth of an UO hierarchy.
        /// </summary>
        public const int MaxDepth = 6;

        /// <summary>
        ///     Length of a code unit between dots.
        /// </summary>
        public const int MaxCodeUnitLength = 2;

        /// <summary>
        ///     Maximum length of the <see cref="Code" /> property.
        /// </summary>
        public const int MaxCodeLength = MaxDepth * MaxCodeUnitLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationUnit" /> class.
        /// </summary>
        public OrganizationUnit()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationUnit" /> class.
        /// </summary>
        /// <param name="companyId">Company's Id or null for host.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="parentId">Parent's Id or null if OU is a root.</param>
        public OrganizationUnit(int? companyId, string displayName, long? parentId = null)
        {
            CompanyId = companyId;
            DisplayName = displayName;
            ParentId = parentId;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="OrganizationUnit" /> class.
        /// </summary>
        /// <param name="companyId">Company's Id or null for host.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="order">Visual order for this ou</param>
        /// <param name="parentId">Parent's Id or null if OU is a root.</param>
        public OrganizationUnit(int? companyId, string displayName, int order, long? parentId = null)
        {
            CompanyId = companyId;
            DisplayName = displayName;
            Order = order;
            ParentId = parentId;
        }

        /// <summary>
        ///     Parent <see cref="OrganizationUnit" />.
        ///     Null, if this OU is root.
        /// </summary>
        [ForeignKey("ParentId")]
        public virtual OrganizationUnit Parent { get; set; }

        /// <summary>
        ///     Parent <see cref="OrganizationUnit" /> Id.
        ///     Null, if this OU is root.
        /// </summary>
        public virtual long? ParentId { get; set; }

        /// <summary>
        ///     Hierarchical Code of this organization unit.
        ///     Example: "01040205".
        ///     This is a unique code for a Company.
        ///     It's changeable if OU hierarchy is changed.
        /// </summary>
        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        /// <summary>
        ///     Display name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxDisplayNameLength)]
        public virtual string DisplayName { get; set; }

        [Required] public virtual int Order { get; set; }

        /// <summary>
        ///     Children of this OU.
        /// </summary>
        public virtual ICollection<OrganizationUnit> Children { get; set; }

        /// <summary>
        ///     CompanyId of this entity.
        /// </summary>
        public virtual int? CompanyId { get; set; }

        /// <summary>
        ///     Creates code for given numbers.
        ///     Example: if numbers are 4,2 then returns "0402";
        /// </summary>
        /// <param name="numbers">Numbers</param>
        public static string CreateCode(params int[] numbers)
        {
            return numbers.IsNullOrEmpty()
                ? null
                : numbers.Select(number => number.ToString(new string('0', MaxCodeUnitLength))).JoinAsString("");
        }

        /// <summary>
        ///     Appends a child code to a parent code.
        ///     Example: if parentCode = "01", childCode = "42" then returns "0142".
        /// </summary>
        /// <param name="parentCode">Parent code. Can be null or empty if parent is a root.</param>
        /// <param name="childCode">Child code.</param>
        public static string AppendCode(string parentCode, string childCode)
        {
            if (childCode.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(childCode), "childCode can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return childCode;
            }

            return parentCode + childCode;
        }

        /// <summary>
        ///     Gets relative code to the parent.
        ///     Example: if code = "19055001" and parentCode = "19" then returns "055001".
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="parentCode">The parent code.</param>
        public static string GetRelativeCode(string code, string parentCode)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (parentCode.IsNullOrEmpty())
            {
                return code;
            }

            if (code.Length == parentCode.Length)
            {
                return null;
            }

            return code.Substring(parentCode.Length);
        }

        /// <summary>
        ///     Calculates next code for given code.
        ///     Example: if code = "0190055001" returns "0190055002".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string CalculateNextCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            string parentCode = GetParentCode(code);
            string lastUnitCode = GetLastUnitCode(code);

            return AppendCode(parentCode, CreateCode(Convert.ToInt32(lastUnitCode) + 1));
        }

        /// <summary>
        ///     Gets the last unit code.
        ///     Example: if code = "195501" returns "01".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetLastUnitCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            return code.Substring(code.Length - MaxCodeUnitLength);
        }

        /// <summary>
        ///     Gets parent code.
        ///     Example: if code = "195501" returns "1955".
        /// </summary>
        /// <param name="code">The code.</param>
        public static string GetParentCode(string code)
        {
            if (code.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(code), "code can not be null or empty.");
            }

            if (code.Length == MaxCodeUnitLength)
            {
                return null;
            }

            return code.Substring(0, code.Length - MaxCodeUnitLength);
        }
    }
}
