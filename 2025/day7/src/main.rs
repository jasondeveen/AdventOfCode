use std::collections;
use std::{fs, time::SystemTime};

mod map;
mod point;
use map::Map;
use point::Point;

fn main() {
    let now = SystemTime::now();

    let args: Vec<String> = std::env::args().collect();
    let raw_input = fs::read_to_string(&args[1]).expect("input couldnt be read.");

    let map = Map::new(raw_input.lines().collect());
    let start_point = map.get_start_point();
    let mut paths = collections::HashMap::new();

    let total_breaks = map.check_path(
        Point {
            x: start_point,
            y: 0,
        },
        &mut paths,
        &mut Vec::new(),
    );

    println!("{:?}", now.elapsed().unwrap());
    println!("Total beam breaks = {}", total_breaks);

    // for entry in &paths {
    //     if *entry.1 > 0 {
    //         println!("{:?}: {}", entry.0, entry.1);
    //     }
    // }

    let mut total_paths = 0;
    let i = 0;
    while total_paths == 0 {
        total_paths = paths
            .get(&Point {
                x: start_point,
                y: i,
            })
            .copied()
            .unwrap_or(0);
    }
    println!(
        "Total possible paths: {}, start_node = {:?}",
        total_paths,
        Point {
            x: start_point,
            y: i
        }
    );
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_check_path() {
        let test_cases = [(
            Point { x: 5, y: 0 },
            Map::new(vec![".....^....", "....^.^...", "...^.^.^.."]),
            6,
        )];

        for (start_point, map, expected_breaks) in test_cases {
            let real_breaks = map.check_path(
                start_point,
                &mut collections::HashMap::new(),
                &mut Vec::new(),
            );
            assert_eq!(real_breaks, expected_breaks);
        }
    }
}
