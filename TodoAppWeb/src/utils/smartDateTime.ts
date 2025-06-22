import dayjs from 'dayjs';
import relativeTime from 'dayjs/plugin/relativeTime';
import advancedFormat from 'dayjs/plugin/advancedFormat';

dayjs.extend(relativeTime);
dayjs.extend(advancedFormat);

export function smartDateTime(dateString: string): string {
  const now = dayjs();
  const date = dayjs(dateString);
  const diffInDays = now.diff(date, 'day');

  const humanized = date.fromNow();
  const formatted = date.format('Do MMMM, YYYY');

  if (diffInDays === 0) return humanized;
  if (diffInDays === 1) return `Yesterday - ${formatted}`;
  if (diffInDays <= 7) return `${humanized} - ${formatted}`;
  return formatted;
}
