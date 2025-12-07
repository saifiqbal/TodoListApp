import React from 'react';
import { FilterType, SortType } from '../types/todo.types';
import { Filter, SortAsc } from 'lucide-react';

interface FilterBarProps {
  filter: FilterType;
  onFilterChange: (filter: FilterType) => void;
  sortBy: SortType;
  onSortChange: (sort: SortType) => void;
  totalCount: number;
  activeCount: number;
  completedCount: number;
}

export const FilterBar: React.FC<FilterBarProps> = ({
  filter,
  onFilterChange,
  sortBy,
  onSortChange,
  totalCount,
  activeCount,
  completedCount,
}) => {
  const filterButtons: { value: FilterType; label: string }[] = [
    { value: 'all', label: `All (${totalCount})` },
    { value: 'active', label: `Active (${activeCount})` },
    { value: 'completed', label: `Completed (${completedCount})` },
    { value: 'overdue', label: 'Overdue' },
  ];

  const sortOptions: { value: SortType; label: string }[] = [
    { value: 'createdAt', label: 'Created Date' },
    { value: 'dueDate', label: 'Due Date' },
    { value: 'title', label: 'Title' },
  ];

  return (
    <div className="bg-white p-4 rounded-lg shadow-md mb-6">
      <div className="flex flex-col md:flex-row gap-4 items-start md:items-center">
        {/* Filter Buttons */}
        <div className="flex items-center gap-2 flex-wrap">
          <Filter size={20} className="text-gray-600" />
          <span className="text-sm font-medium text-gray-700">Filter:</span>
          <div className="flex gap-2">
            {filterButtons.map((btn) => (
              <button
                key={btn.value}
                onClick={() => onFilterChange(btn.value)}
                className={`px-4 py-2 rounded-lg text-sm font-medium transition ${
                  filter === btn.value
                    ? 'bg-purple-600 text-white'
                    : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                }`}
              >
                {btn.label}
              </button>
            ))}
          </div>
        </div>

        {/* Sort Dropdown */}
        <div className="flex items-center gap-2 md:ml-auto">
          <SortAsc size={20} className="text-gray-600" />
          <span className="text-sm font-medium text-gray-700">Sort by:</span>
          <select
            value={sortBy}
            onChange={(e) => onSortChange(e.target.value as SortType)}
            className="px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-purple-500 focus:border-transparent bg-white"
          >
            {sortOptions.map((option) => (
              <option key={option.value} value={option.value}>
                {option.label}
              </option>
            ))}
          </select>
        </div>
      </div>
    </div>
  );
};