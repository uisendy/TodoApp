import { useSelector } from 'react-redux';
import { RootState } from '../app/store';
import PageBreadcrumb from '../components/common/PageBreadCrumb';
import UserMetaCard from '../components/UserProfile/UserMetaCard';
import UserInfoCard from '../components/UserProfile/UserInfoCard';
import PageMeta from '../components/common/PageMeta';

export default function UserProfiles() {
  const user = useSelector((state: RootState) => state.auth.user);

  console.log('user data', user);

  return (
    <>
      <PageMeta
        title="TodoIO - Profile"
        description="Check and Update your Profile"
      />
      <PageBreadcrumb pageTitle="Profile" />
      <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] lg:p-6">
        <h3 className="mb-5 text-lg font-semibold text-gray-800 dark:text-white/90 lg:mb-7">
          Profile
        </h3>
        <div className="space-y-6">
          <UserMetaCard
            name={`${user?.firstName || ''} ${user?.lastName || ''}`}
          />
          <UserInfoCard
            firstName={user?.firstName || ''}
            lastName={user?.lastName || ''}
            bio={user?.bio || 'No bio provided'}
            email={user?.email || ''}
            phone={user?.phone || 'N/A'}
          />
        </div>
      </div>
    </>
  );
}
