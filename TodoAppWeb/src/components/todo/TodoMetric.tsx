import React from 'react';
import { MdFullscreen } from 'react-icons/md';

interface TodoMetricProps {
  label: string;
  metric: string;
  icon: React.ReactNode;
}
export default function TodoMetric({ label, metric, icon }: TodoMetricProps) {
  return (
    <div className="rounded-2xl border border-gray-200 bg-white p-5 dark:border-gray-800 dark:bg-white/[0.03] md:p-6">
      <div className="flex items-center justify-center w-12 h-12 bg-gray-100 rounded-xl dark:bg-gray-800">
        {icon}
      </div>

      <div className="flex items-end justify-between mt-3">
        <div>
          <span className="text-sm text-gray-500 dark:text-gray-400">
            {label}
          </span>
          <h4 className="mt-1 font-bold text-gray-800 text-title-sm dark:text-white/90">
            {metric}
          </h4>
        </div>
      </div>
    </div>
  );
}
