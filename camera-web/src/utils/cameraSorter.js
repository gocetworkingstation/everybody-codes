/**
 * Sort cameras into columns based on divisibility rules
 * 
 * Rules:
 * 1. If number is divisible by 3, then it should go in the first column.
 * 2. If number is divisible by 5, then it should go in the second column.
 * 3. If number is divisible by 3 and divisible by 5, then it should go in the third column.
 * 4. If number is not divisible by 3 and is not divisible by 5, then it should go in the last column.
 * 
 * @param {Array} cameras - Array of camera objects
 * @returns {Object} Object with sorted cameras in columns
 */
export const sortCamerasIntoColumns = (cameras) => {
  const columns = {
    column1: [], // Divisible by 3 (but not by 5)
    column2: [], // Divisible by 5 (but not by 3)
    column3: [], // Divisible by both 3 and 5
    column4: [], // Not divisible by 3 or 5
  };

  cameras.forEach(camera => {
    const number = camera.number;
    const divisibleBy3 = number % 3 === 0;
    const divisibleBy5 = number % 5 === 0;

    if (divisibleBy3 && divisibleBy5) {
      // Rule 3: Divisible by both 3 and 5
      columns.column3.push(camera);
    } else if (divisibleBy3) {
      // Rule 1: Divisible by 3 (but not 5)
      columns.column1.push(camera);
    } else if (divisibleBy5) {
      // Rule 2: Divisible by 5 (but not 3)
      columns.column2.push(camera);
    } else {
      // Rule 4: Not divisible by 3 or 5
      columns.column4.push(camera);
    }
  });

  return columns;
};

/**
 * Get column title based on column key
 * @param {string} columnKey - Column key (column1, column2, etc.)
 * @returns {string} Human-readable column title
 */
export const getColumnTitle = (columnKey) => {
  const titles = {
    column1: 'Divisible by 3',
    column2: 'Divisible by 5', 
    column3: 'Divisible by 3 & 5',
    column4: 'Other Numbers',
  };
  
  return titles[columnKey] || 'Unknown';
};

/**
 * Get column description based on column key
 * @param {string} columnKey - Column key
 * @returns {string} Column description
 */
export const getColumnDescription = (columnKey) => {
  const descriptions = {
    column1: 'Numbers divisible by 3 (but not by 5)',
    column2: 'Numbers divisible by 5 (but not by 3)',
    column3: 'Numbers divisible by both 3 and 5',
    column4: 'Numbers not divisible by 3 or 5',
  };
  
  return descriptions[columnKey] || '';
};
