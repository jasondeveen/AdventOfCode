use std::{env, fs, time::SystemTime};

#[derive(Debug, PartialEq, Clone)]
struct Range {
    start: u64,
    end: u64,
}

#[derive(Debug, PartialEq)]
struct HyperRange {
    pub ranges: Vec<Range>,
}

impl HyperRange {
    fn build(ranges: &Vec<Range>) -> HyperRange {
        let own_ranges: Vec<Range> = ranges.clone();

        let mut hyper_range = HyperRange { ranges: own_ranges };

        hyper_range.fixup_sorted();

        hyper_range
    }

    fn fixup_sorted(&mut self) {
        // 1 sort the ranges based on the start
        // 2 if the end of a range is larger than the start of the next range, merge the ranges

        self.ranges.sort_by_key(|r| r.start);

        let mut i = 0;

        while i + 1 < self.ranges.len() {
            let opt = self.ranges[i].merge(&self.ranges[i + 1]);
            match opt {
                Some(r) => {
                    self.ranges[i] = r;
                    self.ranges.remove(i + 1);
                }
                None => i += 1,
            }
        }
    }
}

impl Range {
    fn contains(&self, num: u64) -> bool {
        if self.start <= num && num <= self.end {
            return true;
        }
        false
    }

    fn merge(&self, other: &Range) -> Option<Range> {
        if self.start > other.end {
            return None;
        }
        if other.start > self.end {
            return None;
        }
        let mut result = Range { start: 0, end: 0 };
        if self.start <= other.start {
            result.start = self.start;
        } else {
            result.start = other.start;
        }

        if self.end >= other.end {
            result.end = self.end;
        } else {
            result.end = other.end;
        }

        Some(result)
    }
}

fn main() {
    let now = SystemTime::now();
    let args: Vec<String> = env::args().collect();

    let contents_raw = fs::read_to_string(&args[1]).expect("Unable to read file from path ");

    let mut contents_raw_split = contents_raw.split("\r\n\r\n");
    let ranges = contents_raw_split.next().expect("No ranges found!");
    let items = contents_raw_split.next().expect("No items found!");

    let ranges: Vec<Range> = get_ranges(ranges).collect();
    let mut item = get_items(items);

    let mut number_of_fresh: u32 = 0;
    let hyper_range = HyperRange::build(&ranges);
    loop {
        let item = item.next();
        if item == None {
            break;
        }

        for range in &hyper_range.ranges {
            if range.contains(item.unwrap()) {
                number_of_fresh += 1;
            }
        }
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("PART 1: Number of spoiled foods: {number_of_fresh}");

    let now = SystemTime::now();
    let mut possible_ids = 0;
    for range in hyper_range.ranges {
        possible_ids += range.end + 1 - range.start; // + 1 because the range's end in inclusive
    }

    println!("{:?}", now.elapsed().unwrap());
    println!("PART 2: Number of possible IDs in all ranges: {possible_ids}");
}

fn get_items(items_raw: &str) -> impl Iterator<Item = u64> {
    items_raw
        .lines()
        .map(|line| line.parse().expect("Cant parse item: {line}"))
}

fn get_ranges(ranges_raw: &str) -> impl Iterator<Item = Range> {
    ranges_raw.lines().map(|line| {
        let lineparts: Vec<&str> = line.split("-").collect();
        Range {
            start: lineparts[0].parse().expect("Cant parse start: {line}"),
            end: lineparts[1].parse().expect("Cant parse end: {line}"),
        }
    })
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_hyper_ranges_build() {
        let test_cases = [(
            vec![
                Range { start: 3, end: 5 },
                Range { start: 10, end: 14 },
                Range { start: 16, end: 20 },
                Range { start: 12, end: 18 },
            ],
            HyperRange {
                ranges: vec![Range { start: 3, end: 5 }, Range { start: 10, end: 20 }],
            },
        )];

        for (test_input, expected_output) in test_cases {
            assert_eq!(HyperRange::build(&test_input), expected_output);
        }
    }

    #[test]
    fn test_range_merge() {
        let test_cases = [
            (
                Range { start: 3, end: 5 },
                Range { start: 6, end: 10 },
                None,
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 4, end: 10 },
                Some(Range { start: 3, end: 10 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 5, end: 10 },
                Some(Range { start: 3, end: 10 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 5, end: 5 },
                Some(Range { start: 3, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 2, end: 4 },
                Some(Range { start: 2, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 2, end: 3 },
                Some(Range { start: 2, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 3, end: 3 },
                Some(Range { start: 3, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 1, end: 7 },
                Some(Range { start: 1, end: 7 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 4, end: 5 },
                Some(Range { start: 3, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 3, end: 4 },
                Some(Range { start: 3, end: 5 }),
            ),
            (
                Range { start: 3, end: 5 },
                Range { start: 3, end: 5 },
                Some(Range { start: 3, end: 5 }),
            ),
        ];

        for (me, other, expected_result) in test_cases {
            let result = me.merge(&other);
            assert_eq!(
                result, expected_result,
                "Comparing ranges failed: real: {:#?}, expected: {:#?}. Inputs: me: {:#?}, other: {:#?}",
                result, expected_result, me, other
            );
        }
    }
}
