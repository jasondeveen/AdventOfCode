use std::{env, fs};

#[derive(Debug, Clone, PartialEq)]
struct Coor {
    x: u32,
    y: u32,
    z: u32,
}

impl Coor {
    fn new(s: String) -> Coor {
        let mut i = s.split(',');
        Coor {
            x: i.next().unwrap().parse::<u32>().unwrap(),
            y: i.next().unwrap().parse::<u32>().unwrap(),
            z: i.next().unwrap().parse::<u32>().unwrap(),
        }
    }
    fn get_distance(&self, c2: &Coor) -> f64 {
        let x_dist = (self.x.abs_diff(c2.x) as u64).pow(2);
        let y_dist = (self.y.abs_diff(c2.y) as u64).pow(2);
        let z_dist = (self.z.abs_diff(c2.z) as u64).pow(2);
        ((x_dist + y_dist + z_dist) as f64).sqrt()
    }
}

fn main() {
    let args: Vec<String> = env::args().collect();
    let coors: Vec<Coor> = fs::read_to_string(&args[1])
        .expect("Couldnt read inputfile")
        .lines()
        .map(|l| l.to_owned())
        .map(|s| Coor::new(s))
        .collect();

    let mut circuits: Vec<Vec<Coor>> = coors.iter().map(|c| vec![c.clone()]).collect();
    let mut sorted_distances = build_distance_map(&coors);

    for _ in 0..10 {
        let shortest = sorted_distances.pop().expect("No more boxes to connect!");
        let index_of_v2 = circuits
            .iter()
            .position(|v| v.contains(&shortest.0.1))
            .expect(format!("Couldnt find circuit containing v2 {:?}", shortest.0.1).as_str());

        let index_of_v1 = circuits
            .iter()
            .position(|v| v.contains(&shortest.0.0))
            .expect(format!("Couldnt find circuit containing v1 {:?}", shortest.0.0).as_str());

        if index_of_v1 == index_of_v2 {
            continue;
        }

        let vals = circuits[index_of_v2].clone();

        for coor in vals {
            circuits[index_of_v1].push(coor);
        }

        circuits.remove(index_of_v2);
    }

    circuits.sort_by(|a, b| a.len().cmp(&b.len()));
    let longest = &circuits[circuits.len() - 1].len();
    let second_longest = &circuits[circuits.len() - 2].len();
    let third_longest = &circuits[circuits.len() - 3].len();
    println!(
        "Lengths of longest circuits: {}, {}, {}. Product = {}",
        third_longest,
        second_longest,
        longest,
        longest * second_longest * third_longest
    );
}

fn build_distance_map(coors: &Vec<Coor>) -> Vec<((Coor, Coor), f64)> {
    let mut distances = Vec::new();
    for c1 in coors {
        for c2 in coors {
            if distances
                .iter()
                .map(|t: &((Coor, Coor), f64)| t.0.clone())
                .collect::<Vec<(Coor, Coor)>>()
                .contains(&(c2.clone(), c1.clone()))
            {
                continue;
            }
            if c1 != c2 {
                distances.push(((c1.clone(), c2.clone()), c1.get_distance(c2)));
            }
        }
    }

    distances.sort_by(|a, b| a.1.partial_cmp(&b.1).unwrap());
    distances.reverse();
    distances
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn test_get_distance() {
        let test_cases = [(
            Coor {
                x: 162,
                y: 817,
                z: 812,
            },
            Coor {
                x: 57,
                y: 618,
                z: 57,
            },
            787.81406435782802671060154068308_f64,
        )];

        for (c1, c2, er) in test_cases {
            let rr = c1.get_distance(&c2);
            assert_eq!(
                rr, er,
                "Expected {er}, got {rr}. c1: {:#?}, c2: {:#?}",
                c1, c2
            );
        }
    }
}
