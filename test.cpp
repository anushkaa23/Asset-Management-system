#include <bits/stdc++.h>
using namespace std;

int longestBalancedSubstring(const string &s) {
    int n = s.size();
    if (n == 0) return 0;

    int ans = 1; // minimum substring length

    const int ALPHABET = 26;

    // Count for single-character runs
    {
        int run = 1;
        for (int i = 1; i < n; i++) {
            if (s[i] == s[i - 1]) run++;
            else run = 1;
            ans = max(ans, run);
        }
    }

    // Precompute all unique letters in the string
    vector<char> letters;
    vector<bool> seen(ALPHABET, false);
    for (char ch : s)
        if (!seen[ch - 'a']) {
            letters.push_back(ch);
            seen[ch - 'a'] = true;
        }

    int m = letters.size(); // number of distinct letters

    // For all subset sizes 2..m
    for (int sz = 2; sz <= m; sz++) {
        // Enumerate all subsets of size sz
        vector<int> idx(sz);
        iota(idx.begin(), idx.end(), 0);
        while (true) {
            vector<char> subset(sz);
            for (int i = 0; i < sz; i++)
                subset[i] = letters[idx[i]];

            // Generalized prefix-difference hashmap
            unordered_map<string,int> first;
            vector<int> counts(sz, 0);
            vector<int> diff(sz - 1, 0);

            string key(sz - 1, 0);
            first[key] = -1;

            for (int i = 0; i < n; i++) {
                auto it = find(subset.begin(), subset.end(), s[i]);
                if (it != subset.end()) {
                    int p = it - subset.begin();
                    counts[p]++;
                    for (int j = 1; j < sz; j++)
                        diff[j - 1] = counts[0] - counts[j];

                    string kstr(diff.begin(), diff.end());
                    if (first.count(kstr))
                        ans = max(ans, i - first[kstr]);
                    else
                        first[kstr] = i;
                } else {
                    // reset when forbidden character encountered
                    fill(counts.begin(), counts.end(), 0);
                    fill(diff.begin(), diff.end(), 0);
                    first.clear();
                    string kstr(sz - 1, 0);
                    first[kstr] = i;
                }
            }

            // Generate next combination
            int i;
            for (i = sz - 1; i >= 0; i--) {
                if (idx[i] != i + m - sz) break;
            }
            if (i < 0) break;
            idx[i]++;
            for (int j = i + 1; j < sz; j++) idx[j] = idx[j - 1] + 1;
        }
    }

    return ans;
}

int main() {
    ios::sync_with_stdio(false);
    cin.tie(nullptr);

    cout << longestBalancedSubstring("aa") << "\n";       // 2
    cout << longestBalancedSubstring("abbac") << "\n";   // 4
    cout << longestBalancedSubstring("aabcc") << "\n";   // 3
    cout << longestBalancedSubstring("aba") << "\n";     // 2
    cout << longestBalancedSubstring("abcbc") << "\n";   // 4
    cout << longestBalancedSubstring("aaabbbccc") << "\n"; // 9
}