using MasterService.Domain.Entities;
using StreamEntity = MasterService.Domain.Entities.Stream;

namespace MasterService.Infrastructure.Repositories
{
    internal static class RepositoryEntityMapper
    {
        public static void AttachQualificationLanguages(IEnumerable<Qualification> qualifications, IEnumerable<QualificationLanguage> languages)
        {
            var languageLookup = languages
                .GroupBy(language => language.QualificationId)
                .ToDictionary(group => group.Key, group => (ICollection<QualificationLanguage>)group.ToList());

            foreach (var qualification in qualifications)
            {
                qualification.QualificationLanguages = languageLookup.TryGetValue(qualification.Id, out var items)
                    ? items
                    : new List<QualificationLanguage>();
            }
        }

        public static void AttachStreamDetails(IEnumerable<StreamEntity> streams, IEnumerable<StreamLanguage> languages, IEnumerable<Qualification> qualifications)
        {
            var languageLookup = languages
                .GroupBy(language => language.StreamId)
                .ToDictionary(group => group.Key, group => (ICollection<StreamLanguage>)group.ToList());
            var qualificationLookup = qualifications.ToDictionary(qualification => qualification.Id);

            foreach (var stream in streams)
            {
                stream.StreamLanguages = languageLookup.TryGetValue(stream.Id, out var items)
                    ? items
                    : new List<StreamLanguage>();

                if (qualificationLookup.TryGetValue(stream.QualificationId, out var qualification))
                {
                    stream.Qualification = qualification;
                }
            }
        }

        public static void AttachSubjectLanguages(IEnumerable<Subject> subjects, IEnumerable<SubjectLanguage> languages)
        {
            var languageLookup = languages
                .GroupBy(language => language.SubjectId)
                .ToDictionary(group => group.Key, group => (ICollection<SubjectLanguage>)group.ToList());

            foreach (var subject in subjects)
            {
                subject.SubjectLanguages = languageLookup.TryGetValue(subject.Id, out var items)
                    ? items
                    : new List<SubjectLanguage>();
            }
        }
    }
}
